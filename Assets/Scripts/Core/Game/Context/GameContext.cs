using System;
using Core.Game.Dto.Snapshots;

namespace Core.Game
{
    public sealed class GameContext
    {
        private readonly IGameDataProvider _dataProvider;
        private bool _isNotifying;

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
            EnsureNotNotifying();

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
            EnsureNotNotifying();
            EnsureInitialized();

            Revision = revision;
            NotifyStateChanged();
        }

        private void EnsureInitialized()
        {
            if (!IsInitialized)
            {
                throw new InvalidOperationException("The initial game snapshot has not been loaded.");
            }
        }

        private void EnsureNotNotifying()
        {
            if (_isNotifying)
            {
                throw new InvalidOperationException("State observers cannot load or commit another state synchronously.");
            }
        }

        private void NotifyStateChanged()
        {
            _isNotifying = true;

            try
            {
                StateChanged?.Invoke();
            }
            finally
            {
                _isNotifying = false;
            }
        }
    }
}
