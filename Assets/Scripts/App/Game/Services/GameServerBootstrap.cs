using System;
using System.Linq;
using Core.Game;
using Core.Game.Cards;
using Core.Game.Dto.Snapshots;
using Core.Game.Galaxy;
using Core.Game.InitialSnapshotFactory;
using Core.Game.Players;
using Core.Game.Server;
using Core.User;
using Network.Game;
using VContainer;

namespace App.Game.Services
{
    public sealed class GameServerBootstrap : IDisposable
    {
        private const long DefaultRevision = 10;

        [Inject] private readonly GameContext _context = null!;
        [Inject] private readonly GalaxyInitialStateFactory _galaxyFactory = null!;
        [Inject] private readonly PlayerInitialStateFactory _playerFactory = null!;
        [Inject] private readonly CardsInitialStateFactory _cardsFactory = null!;
        [Inject] private readonly PhaseInitialStateFactory _phaseFactory = null!;
        [Inject] private readonly ServerUsersRepository _users = null!;
        [Inject] private readonly IUsersColorProvider _colorProvider = null!;
        [Inject] private readonly ServerClientRevisionTracker _tracker = null!;
        [Inject] private readonly GameRevisionAckNetwork _revisionAck = null!;
        [Inject] private readonly ServerPhaseScheduler _serverPhaseScheduler = null!;
        [Inject] private readonly GameStartupNetwork _startupNetwork = null!;

        private bool _disposed;

        public void Init()
        {
            Init(Environment.TickCount);
        }

        public void Dispose()
        {
            _disposed = true;
            _tracker.ClientRevisionChanged -= OnClientRevisionChanged;
        }
        
        private void Init(int seed)
        {
            var participants = _users.Users.Select(user => new GameParticipantData(
                user.ClientId, user.Name, user.SeatNumber, _colorProvider.GetColor(user.ColorId))).ToList();
            var playersSnapshot = _playerFactory.Create(participants);
            var galaxySnapshot = _galaxyFactory.Create(playersSnapshot.Players);
            var cardsSnapshot = _cardsFactory.Create(playersSnapshot.Players, seed);
            var phaseSnapshot = _phaseFactory.Create(DefaultRevision);

            var snapshot = new GameSnapshot
            {
                GamePlayers = playersSnapshot,
                Cards = cardsSnapshot,
                Galaxy = galaxySnapshot,
                Encounter = new EncounterSnapshot(),
                Phase = phaseSnapshot
            };

            _context.LoadSnapshot(DefaultRevision, snapshot);
            if (_disposed)
            {
                return;
            }

            _tracker.Initialize(_context);
            _revisionAck.SetServerTracker(_tracker);
            _tracker.ClientRevisionChanged += OnClientRevisionChanged;
        }

        private void OnClientRevisionChanged(ulong clientId, long revision)
        {
            if (_disposed || _context.IsOperationFaulted ||
                !_startupNetwork.IsWaitingForInitialAcknowledgements ||
                !_tracker.AreAllClientsAtRevision(DefaultRevision))
            {
                return;
            }

            _tracker.ClientRevisionChanged -= OnClientRevisionChanged;
            _serverPhaseScheduler.Start();
            _startupNetwork.MarkGameRunning();
        }
    }
}
