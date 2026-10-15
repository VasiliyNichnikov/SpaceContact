using System;
using Core.Game.Dto.Snapshots;
using Core.Game.Validation;
using Logs;

namespace Core.Game
{
    public sealed class GameContext
    {
        private readonly IGameDataProvider _dataProvider;
        private bool _isNotifying;
        private bool _isApplyingServerOperation;

        public GameContext(IGameServerTime time, IGameDataProvider dataProvider, GameContextRole role)
        {
            Time = time;
            _dataProvider = dataProvider;
            Role = role;
        }
        
        public event Action? StateChanged;

        public PlayersContext Players { get; } = new();

        public GalaxyContext Galaxy { get; } = new();

        public CardsContext Cards { get; } = new();

        public PhaseContext Phase { get; } = new();

        public EncounterContext Encounter { get; } = new();

        public ulong LocalPlayerId =>
            _dataProvider.LocalPlayerId;

        public IGameServerTime Time { get; }

        public long Revision { get; private set; }

        public GameContextRole Role { get; }

        public bool IsInitialized { get; private set; }

        public bool IsOperationFaulted { get; private set; }

        public GamePlayerData? LocalPlayerState
        {
            get
            {
                if (!IsInitialized || Role != GameContextRole.Client || !Players.IsPlayerExist(LocalPlayerId))
                {
                    return null;
                }

                return Players.Get(LocalPlayerId);
            }
        }
        
        public void LoadSnapshot(long revision, GameSnapshot snapshot)
        {
            EnsureAvailable();
            if (IsInitialized && revision < Revision)
            {
                throw new StateValidationException("$.Revision", "An older snapshot cannot replace the current state.");
            }

            StateValidator.Validate(snapshot, Role, LocalPlayerId, revision);

            var players = new PlayersContext();
            var galaxy = new GalaxyContext();
            var cards = new CardsContext();
            var encounter = new EncounterContext();
            var phase = new PhaseContext();

            players.LoadSnapshot(snapshot.GamePlayers);
            galaxy.LoadSnapshot(snapshot.Galaxy);
            cards.LoadSnapshot(snapshot.Cards);
            encounter.LoadSnapshot(snapshot.Encounter);
            phase.LoadSnapshot(snapshot.Phase);

            // Keep section instances and their subscriptions; only replace prepared data.
            Players.ReplaceState(players);
            Galaxy.ReplaceState(galaxy);
            Cards.ReplaceState(cards);
            Encounter.ReplaceState(encounter);
            Phase.ReplaceState(phase);

            Revision = revision;
            IsInitialized = true;
            NotifyStateChanged();
        }

        public GameSnapshot ToSnapshot()
        {
            EnsureInitialized();
            if (IsOperationFaulted || (_isApplyingServerOperation && !_isNotifying))
            {
                throw new InvalidOperationException("An incomplete server operation cannot be exposed as a snapshot.");
            }

            return new GameSnapshot
            {
                GamePlayers = Players.ToSnapshot(),
                Galaxy = Galaxy.ToSnapshot(),
                Cards = Cards.ToSnapshot(),
                Encounter = Encounter.ToSnapshot(),
                Phase = Phase.ToSnapshot()
            };
        }

        public void CommitRevision(long revision)
        {
            EnsureAvailable();
            EnsureInitialized();

            if (Role != GameContextRole.Client)
            {
                throw new InvalidOperationException("Server revisions are committed only by the action executor.");
            }

            EnsureNextRevision(revision);
            Revision = revision;
            NotifyStateChanged();
        }

        internal void BeginServerOperation()
        {
            EnsureAvailable();
            EnsureInitialized();
            if (Role != GameContextRole.Server)
            {
                throw new InvalidOperationException("Only a server context can execute authoritative actions.");
            }

            _isApplyingServerOperation = true;
        }

        internal void CommitServerOperation(long revision)
        {
            if (!_isApplyingServerOperation || IsOperationFaulted)
            {
                throw new InvalidOperationException("There is no active server operation to commit.");
            }

            EnsureNextRevision(revision);
            Revision = revision;
            NotifyStateChanged();
        }

        internal void FaultServerOperation()
        {
            IsOperationFaulted = true;
        }

        internal void EndServerOperation()
        {
            _isApplyingServerOperation = false;
        }

        private void EnsureNextRevision(long revision)
        {
            if (Revision == long.MaxValue || revision != Revision + 1)
            {
                throw new InvalidOperationException("A committed operation must advance the revision by exactly one.");
            }
        }

        private void EnsureInitialized()
        {
            if (!IsInitialized)
            {
                throw new InvalidOperationException("The initial game snapshot has not been loaded.");
            }
        }

        private void EnsureAvailable()
        {
            if (_isNotifying || _isApplyingServerOperation)
            {
                throw new InvalidOperationException("Another state operation is still in progress.");
            }

            if (IsOperationFaulted)
            {
                throw new InvalidOperationException("A server operation failed; this game session cannot continue.");
            }
        }

        private void NotifyStateChanged()
        {
            _isNotifying = true;

            try
            {
                var observers = StateChanged?.GetInvocationList();
                if (observers is null)
                {
                    return;
                }

                foreach (Action observer in observers)
                {
                    try
                    {
                        observer();
                    }
                    catch (Exception exception)
                    {
                        // State is committed. A failing observer must not turn success into a retryable failure.
                        Logger.Error($"Game state observer failed after revision {Revision}: {exception}");
                    }
                }
            }
            finally
            {
                _isNotifying = false;
            }
        }
    }
}
