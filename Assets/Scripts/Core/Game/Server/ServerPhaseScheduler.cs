using System;
using System.Linq;
using Core.Game.Actions;
using Core.Game.Dto.Snapshots;
using Core.Helpers;
using Logs;

namespace Core.Game.Server
{
    public sealed class ServerPhaseScheduler : IDisposable
    {
        private readonly GameContext _context;
        private readonly GameActionExecutor _executor;
        private readonly IServerEventBroadcaster _broadcaster;
        private readonly IPhaseEntryAcknowledgements _acknowledgements;
        private readonly ServerDestinyTargetSelector _targetSelector;

        private long? _phaseRevision;
        private long? _entryStateRevision;
        private long? _blockedStateRevision;
        private bool _barrierPassed;
        private bool _started;
        private bool _disposed;
        private bool _updating;

        public ServerPhaseScheduler(
            GameContext context,
            GameActionExecutor executor,
            IServerEventBroadcaster broadcaster,
            IPhaseEntryAcknowledgements acknowledgements,
            ServerDestinyTargetSelector targetSelector)
        {
            _context = context;
            _executor = executor;
            _broadcaster = broadcaster;
            _acknowledgements = acknowledgements;
            _targetSelector = targetSelector;
        }

        /// <summary>
        /// Bootstrap calls this after loading and synchronizing the initial snapshot.
        /// Repeated calls are harmless; a disposed scheduler cannot be restarted.
        /// </summary>
        public void Start()
        {
            if (!CanStart())
            {
                return;
            }

            _started = true;
            _context.Time.Subscribe(Update);
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            if (_started)
            {
                _context.Time.Unsubscribe(Update);
            }
        }

        private bool CanRun() =>
            !_disposed && _context.Role == GameContextRole.Server && _context.IsInitialized &&
            !_context.IsOperationFaulted && _context.Players.All.Count > 0;

        private bool CanStart() =>
            !_started && CanRun();

        private bool CanUpdate() =>
            _started && CanRun() && !_updating && !_context.IsBusy &&
            _blockedStateRevision != _context.Revision;

        private void Update(double deltaTimeInSeconds)
        {
            // StateChanged runs inside the executor's commit. Only the clock drives new operations,
            // and even a synchronous clock callback must wait until that commit has finished.
            if (!CanUpdate())
            {
                return;
            }

            _updating = true;
            try
            {
                Advance();
            }
            finally
            {
                _updating = false;
            }
        }

        private void Advance()
        {
            var phase = _context.Phase;
            if (phase.Type == GamePhaseType.None)
            {
                return;
            }

            if (_phaseRevision != phase.Revision)
            {
                _phaseRevision = phase.Revision;
                _entryStateRevision = null;
                _barrierPassed = false;
            }

            if (_entryStateRevision == null)
            {
                if (!ApplyEntryEffects())
                {
                    return;
                }

                // Freeze the revision covering both the phase change and its input effects.
                // Timer/readiness commits must never move this barrier forward.
                _entryStateRevision = _context.Revision;
            }

            if (_disposed)
            {
                return;
            }

            if (!_barrierPassed)
            {
                var participants = _context.Players.All.Select(player => player.Id).ToArray();
                _barrierPassed = _acknowledgements.AreAllPlayersAtEntry(participants, phase.Revision, _entryStateRevision.Value);
                if (!_barrierPassed || _disposed)
                {
                    return;
                }
            }

            if (_context.PhaseDurations.TryGetDuration(phase.Type, out var duration))
            {
                if (!phase.EndTimeInSeconds.HasValue)
                {
                    Execute(new StartPhaseTimerGameAction(phase.Type, phase.Revision, duration));
                    return;
                }

                if (!HasDeadlineElapsed())
                {
                    UpdateReadyTimer();
                    return;
                }
            }

            if (!TransitionPhaseHelper.TryGetNextPhase(phase.Type, out var nextPhase))
            {
                return;
            }

            if (phase.Type == GamePhaseType.Destiny && !CompleteDestiny())
            {
                return;
            }

            if (!_disposed && _context.Revision < long.MaxValue)
            {
                // The new phase's entry effects are processed on the next tick: at most one exit per tick.
                Execute(new TransitionToNextPhaseGameAction(phase.Type, phase.Revision, nextPhase, _context.Revision + 1));
            }
        }

        private bool ApplyEntryEffects()
        {
            return _context.Phase.Type switch
            {
                GamePhaseType.FirstMove => Execute(new StartEncounterGameAction()),
                GamePhaseType.Destiny => _context.Cards.ActiveDestinyCard is not null ||
                    Execute(new ChooseDestinyCardGameAction()),
                _ => true
            };
        }

        private bool HasDeadlineElapsed()
        {
            var phase = _context.Phase;
            var now = _context.Time.ServerTimeInSeconds;
            return now >= phase.EndTimeInSeconds ||
                (now >= phase.ReadyTransitionTimeInSeconds && AreAllPlayersReady());
        }

        private bool AreAllPlayersReady() =>
            _context.Players.All.Count > 0 && _context.Players.All.All(player => player.IsReady);

        private void UpdateReadyTimer()
        {
            var phase = _context.Phase;

            if (AreAllPlayersReady())
            {
                if (phase.ReadyTransitionTimeInSeconds == null)
                {
                    Execute(new StartReadyTransitionTimerGameAction(phase.Revision));
                }
            }
            else if (phase.ReadyTransitionTimeInSeconds != null)
            {
                Execute(new CancelReadyTransitionTimerGameAction(phase.Revision));
            }
        }

        private bool CompleteDestiny()
        {
            if (_context.Encounter.HasDefender && _context.Encounter.HasPlanetToAttack)
            {
                return true;
            }

            var target = _targetSelector.SelectTarget(_context);
            if (target is not null)
            {
                return ExecuteAutomatic(target);
            }

            var card = _context.Cards.ActiveDestinyCard;
            var aggressorId = _context.Encounter.AggressorPlayerId;
            if (card is not null && !card.IsJoker && !card.SpecificCardId.HasValue &&
                aggressorId.HasValue && card.SelectedPlayerId == aggressorId)
            {
                // At most one skip per tick. Every successful skip consumes a card; an exhausted deck
                // is rejected without discarding the last active card and blocks retries at this revision.
                var action = new SkipDestinyCardGameAction(aggressorId.Value, _context.Phase.Revision, _context.Cards.DestinyDeckCount);
                ExecuteAutomatic(action);
                return false;
            }

            BlockAtCurrentState("Destiny has no legal target. Waiting for a state change.");
            return false;
        }

        private bool Execute(ServerGameAction action)
        {
            if (_disposed)
            {
                return false;
            }

            return HandleExecutionResult(action, _executor.ExecuteServerAction(action));
        }

        private bool ExecuteAutomatic(AutomaticClientGameAction action)
        {
            if (_disposed)
            {
                return false;
            }

            return HandleExecutionResult(action, _executor.ExecuteAutomaticAction(action));
        }

        private bool HandleExecutionResult(BaseGameAction action, GameActionExecutionResult result)
        {
            if (!result.IsSuccess)
            {
                BlockAtCurrentState($"{action.GetType().Name} rejected: {result.ErrorMessage}");
                return false;
            }

            foreach (var (recipientId, batch) in result.BatchesByRecipient)
            {
                if (_disposed)
                {
                    break;
                }

                try
                {
                    _broadcaster.SendEvent(recipientId, batch);
                }
                catch (Exception exception)
                {
                    // The operation is already committed. Continue delivery to the other participants;
                    // recovery uses snapshot synchronization, not re-execution of the action.
                    Logger.Error($"Failed to deliver revision {result.Revision} to player {recipientId}: {exception}");
                }
            }

            return true;
        }

        private void BlockAtCurrentState(string reason)
        {
            _blockedStateRevision = _context.Revision;
            Logger.Warning(reason);
        }
    }
}
