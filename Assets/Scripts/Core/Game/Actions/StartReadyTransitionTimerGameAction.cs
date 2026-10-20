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
    public sealed class StartReadyTransitionTimerGameAction : ServerGameAction
    {
        public StartReadyTransitionTimerGameAction(long phaseRevision)
        {
            PhaseRevision = phaseRevision;
        }
        
        public enum Status
        {
            InvalidPhaseEntry = 0,
            PhaseTimerNotRunning = 1,
            ParticipantsNotReady = 2,
            ReadyTimerAlreadyRunning = 3,
            PhaseDeadlineElapsed = 4,
            UnrepresentableDeadline = 5
        }

        [JsonProperty] public long PhaseRevision { get; }

        public override IActionValidator GetValidator()
        {
            return new Validator(PhaseRevision);
        }

        protected override IReadOnlyList<IGameEvent> ExecuteCore(GameContext context, GameActor actor)
        {
            var readyTime = context.Time.ServerTimeInSeconds + context.PhaseDurations.ReadyTransitionDurationInSeconds;
            return new IGameEvent[] { new ApplyStartReadyTransitionTimerGameEvent(readyTime) };
        }

        private sealed class Validator : IActionValidator
        {
            private const double DeadlineToleranceInSeconds = 0.000001;

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

                if (!phase.EndTimeInSeconds.HasValue)
                {
                    return ValidationResult.Failed((int)Status.PhaseTimerNotRunning,
                        "The phase timer has not started.");
                }

                if (context.Players.All.Count == 0 || !context.Players.All.All(player => player.IsReady))
                {
                    return ValidationResult.Failed((int)Status.ParticipantsNotReady,
                        "The ready timer requires all participants to be ready.");
                }

                if (phase.ReadyTransitionTimeInSeconds.HasValue)
                {
                    return ValidationResult.Failed(
                        (int)Status.ReadyTimerAlreadyRunning,
                        "The ready timer is already running.");
                }

                var now = context.Time.ServerTimeInSeconds;
                var endTime = phase.EndTimeInSeconds.Value;

                if (now >= endTime)
                {
                    return ValidationResult.Failed((int)Status.PhaseDeadlineElapsed,
                        "The phase deadline has elapsed.");
                }

                var durationInSeconds = context.PhaseDurations.ReadyTransitionDurationInSeconds;
                var readyTime = now + durationInSeconds;

                return Math.Abs(readyTime - now - durationInSeconds) > DeadlineToleranceInSeconds
                    ? ValidationResult.Failed((int)Status.UnrepresentableDeadline,
                        "The ready deadline cannot represent the required duration.")
                    : ValidationResult.Ok;
            }
        }
    }
}