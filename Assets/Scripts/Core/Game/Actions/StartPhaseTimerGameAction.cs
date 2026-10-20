using System;
using System.Collections.Generic;
using Core.Game.Dto.Snapshots;
using Core.Game.Events;
using Core.Helpers;
using Newtonsoft.Json;

namespace Core.Game.Actions
{
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class StartPhaseTimerGameAction : ServerGameAction
    {
        public StartPhaseTimerGameAction(GamePhaseType phaseType, long phaseRevision, double durationInSeconds)
        {
            PhaseType = phaseType;
            PhaseRevision = phaseRevision;
            DurationInSeconds = durationInSeconds;
        }
        
        public enum Status
        {
            MissingParticipants = 0,
            InvalidPhaseEntry = 1,
            TimerAlreadyStarted = 2,
            UnrepresentableDeadline = 3
        }

        [JsonProperty] public GamePhaseType PhaseType { get; }

        [JsonProperty] public long PhaseRevision { get; }

        [JsonProperty] public double DurationInSeconds { get; }

        public override IActionValidator GetValidator()
        {
            return new Validator(PhaseType, PhaseRevision, DurationInSeconds);
        }

        protected override IReadOnlyList<IGameEvent> ExecuteCore(GameContext context, GameActor actor)
        {
            var endTime = context.Time.ServerTimeInSeconds + DurationInSeconds;
            return new IGameEvent[] { new PhaseTimerStartedGameEvent(PhaseRevision, endTime) };
        }

        private sealed class Validator : IActionValidator
        {
            private const double DeadlineToleranceInSeconds = 0.000001;

            private readonly GamePhaseType _phaseType;
            private readonly long _phaseRevision;
            private readonly double _durationInSeconds;

            public Validator(GamePhaseType phaseType, long phaseRevision,
                double durationInSeconds)
            {
                _phaseType = phaseType;
                _phaseRevision = phaseRevision;
                _durationInSeconds = durationInSeconds;
            }

            public ValidationResult Check(GameContext context, GameActor actor)
            {
                if (context.Players.All.Count == 0)
                {
                    return ValidationResult.Failed((int)Status.MissingParticipants,
                        "A phase timer requires participants.");
                }

                var phase = context.Phase;
                if (phase.Type != _phaseType || phase.Revision != _phaseRevision ||
                    !TransitionPhaseHelper.IsTimedPhase(_phaseType))
                {
                    return ValidationResult.Failed((int)Status.InvalidPhaseEntry,
                        "The phase entry is stale or does not support a timer.");
                }

                if (phase.EndTimeInSeconds.HasValue || phase.ReadyTransitionTimeInSeconds.HasValue)
                {
                    return ValidationResult.Failed((int)Status.TimerAlreadyStarted,
                        "A timer has already started for this phase entry.");
                }

                var now = context.Time.ServerTimeInSeconds;
                var endTime = now + _durationInSeconds;
                if (_durationInSeconds > 0 && Math.Abs(endTime - now) < DeadlineToleranceInSeconds)
                {
                    return ValidationResult.Failed((int)Status.UnrepresentableDeadline,
                        "The phase deadline is too close to the current server time.");
                }

                return ValidationResult.Ok;
            }
        }
    }
}