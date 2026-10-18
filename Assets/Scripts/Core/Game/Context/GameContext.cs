using System;
using Core.Game.Dto.Snapshots;
using Core.Game.Validation;
using Logs;

namespace Core.Game
{
    public sealed class GameContext
    {
        private readonly IGameDataProvider _dataProvider;
        private GameContextState _state;

        public GameContext(IGameServerTime time, IGameDataProvider dataProvider, GameContextRole role)
        {
            Time = time;
            _dataProvider = dataProvider;
            _state = new GameContextState(role);
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

        public long Revision => _state.Revision;

        public GameContextRole Role => _state.Role;

        public bool IsInitialized => _state.IsInitialized;

        public bool IsOperationFaulted => _state.IsOperationFaulted;

        public bool IsSynchronizationRequired => _state.IsSynchronizationRequired;

        public GamePlayerData? LocalPlayerState
        {
            get
            {
                if (!IsInitialized || Role != GameContextRole.Client || IsSynchronizationRequired || !Players.IsPlayerExist(LocalPlayerId))
                {
                    return null;
                }

                return Players.Get(LocalPlayerId);
            }
        }
        
        internal bool IsBusy => _state.IsBusy;

        public void LoadSnapshot(long revision, GameSnapshot snapshot)
        {
            EnsureAvailable();
            if (IsInitialized && revision < Revision)
            {
                throw new StateValidationException("$.Revision", "An older snapshot cannot replace the current state.");
            }

            if (_state.RequiredSnapshotRevision.HasValue && revision < _state.RequiredSnapshotRevision.Value)
            {
                throw new StateValidationException("$.Revision", "The snapshot does not cover the revisions awaiting synchronization.");
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

            _state.LoadSnapshot(revision);
            NotifyStateChanged();
        }

        public GameSnapshot ToSnapshot()
        {
            EnsureInitialized();
            if (!_state.CanCreateSnapshot)
            {
                throw new InvalidOperationException("An incomplete or unsynchronized state cannot be exposed as a snapshot.");
            }

            return CreateSnapshot();
        }

        public void CommitRevision(long revision)
        {
            EnsureAvailable();
            EnsureInitialized();

            if (Role != GameContextRole.Client)
            {
                throw new InvalidOperationException("Server revisions are committed only by the action executor.");
            }

            EnsureClientSynchronized();
            EnsureNextRevision(revision);
            _state.CommitRevision(revision);
            NotifyStateChanged();
        }

        internal void BeginClientBatch()
        {
            EnsureAvailable();
            if (Role != GameContextRole.Client)
            {
                throw new InvalidOperationException("Only a client context can apply received event batches.");
            }

            _state.BeginClientBatch();
        }

        internal void CommitClientBatch(long revision)
        {
            if (!_state.IsApplyingClientBatch)
            {
                throw new InvalidOperationException("There is no active client batch to commit.");
            }

            EnsureClientSynchronized();
            EnsureNextRevision(revision);
#if UNITY_EDITOR || DEBUG
            StateValidator.Validate(CreateSnapshot(), Role, LocalPlayerId, revision);
#endif
            _state.CommitRevision(revision);
            NotifyStateChanged();
        }

        internal void RequireClientSnapshot(long revision)
        {
            _state.RequireClientSnapshot(revision);
        }

        internal void EndClientBatch()
        {
            _state.EndOperation();
        }

        internal void BeginServerOperation()
        {
            EnsureAvailable();
            EnsureInitialized();
            if (Role != GameContextRole.Server)
            {
                throw new InvalidOperationException("Only a server context can execute authoritative actions.");
            }

            _state.BeginServerOperation();
        }

        internal void CommitServerOperation(long revision)
        {
            if (!_state.IsApplyingServerOperation || IsOperationFaulted)
            {
                throw new InvalidOperationException("There is no active server operation to commit.");
            }

            EnsureNextRevision(revision);
            _state.CommitRevision(revision);
            NotifyStateChanged();
        }

        internal void FaultServerOperation()
        {
            _state.FaultServerOperation();
        }

        internal void EndServerOperation()
        {
            _state.EndOperation();
        }

        private GameSnapshot CreateSnapshot()
        {
            return new GameSnapshot
            {
                GamePlayers = Players.ToSnapshot(),
                Galaxy = Galaxy.ToSnapshot(),
                Cards = Cards.ToSnapshot(),
                Encounter = Encounter.ToSnapshot(),
                Phase = Phase.ToSnapshot()
            };
        }

        private void EnsureClientSynchronized()
        {
            EnsureInitialized();
            if (IsSynchronizationRequired)
            {
                throw new InvalidOperationException("A current snapshot is required before committing client state.");
            }
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
            if (_state.IsBusy)
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
            _state.BeginNotification();

            try
            {
                var observers = StateChanged?.GetInvocationList();
                if (observers is null)
                {
                    return;
                }

                foreach (var action in observers)
                {
                    var observer = (Action)action;
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
                _state.EndNotification();
            }
        }
    }
}
