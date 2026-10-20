using System;
using System.Collections.Generic;
using System.Linq;
using Core.Game.Events;
using Core.Helpers;
using Newtonsoft.Json;

namespace Core.Game.Actions
{
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class CancelReadyTransitionTimerGameAction : ServerGameAction
    {
        public CancelReadyTransitionTimerGameAction(long phaseRevision)
        {
            PhaseRevision = phaseRevision;
        }
        
        public enum Status
        {
            InvalidPhaseEntry = 0,
            TimersNotRunning = 1,
            PhaseDeadlineElapsed = 2,
            AllParticipantsReady = 3
        }

        [JsonProperty]
        public long PhaseRevision { get; }

        public override IActionValidator GetValidator()
        {
            return new Validator(PhaseRevision);
        }

        protected override IReadOnlyList<IGameEvent> ExecuteCore(GameContext context, GameActor actor)
        {
            return new IGameEvent[] { new ApplyPhaseTransitionTimerCancellationGameEvent() };
        }
        
        private sealed class Validator : IActionValidator
        {
            private readonly long _phaseRevision;

            public Validator(long phaseRevision)
            {
                _phaseRevision = phaseRevision;
            }

            public ValidationResult Check(GameContext context, GameActor actor)
            {
                var phase = context.Phase;

                if (phase.Revision != _phaseRevision || !TransitionPhaseHelper.IsTimedPhase(phase.Type))
                {
                    return ValidationResult.Failed((int)Status.InvalidPhaseEntry,
                        "The phase entry is stale or does not support a ready timer.");
                }

                if (!phase.EndTimeInSeconds.HasValue || !phase.ReadyTransitionTimeInSeconds.HasValue)
                {
                    return ValidationResult.Failed((int)Status.TimersNotRunning,
                        "The phase and ready timers must be running.");
                }

                var now = context.Time.ServerTimeInSeconds;
                var endTime = phase.EndTimeInSeconds.Value;

                if (now >= endTime)
                {
                    return ValidationResult.Failed((int)Status.PhaseDeadlineElapsed,
                        "The phase deadline has elapsed and cannot be cancelled.");
                }

                if (context.Players.All.Count > 0 && context.Players.All.All(player => player.IsReady))
                {
                    return ValidationResult.Failed((int)Status.AllParticipantsReady,
                        "The ready timer cannot be cancelled while all participants are ready.");
                }

                return ValidationResult.Ok;
            }
        }
    }
}
