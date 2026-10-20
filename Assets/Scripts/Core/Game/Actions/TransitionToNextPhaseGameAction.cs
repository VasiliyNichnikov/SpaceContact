using System;
using System.Collections.Generic;
using System.Linq;
using Core.Game.Dto.Snapshots;
using Core.Game.Events;
using Core.Helpers;
using Newtonsoft.Json;

namespace Core.Game.Actions
{
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class TransitionToNextPhaseGameAction : ServerGameAction
    {
        public TransitionToNextPhaseGameAction(GamePhaseType currentPhaseType, long currentPhaseRevision,
            GamePhaseType nextPhaseType, long phaseRevision)
        {
            CurrentPhaseType = currentPhaseType;
            CurrentPhaseRevision = currentPhaseRevision;
            NextPhaseType = nextPhaseType;
            PhaseRevision = phaseRevision;
        }
        
        public enum Status
        {
            StalePhase = 0,
            InvalidRevision = 1,
            UnsupportedTransition = 2,
            MissingParticipants = 3,
            TransitionNotDue = 4
        }

        [JsonProperty] public GamePhaseType CurrentPhaseType { get; }

        [JsonProperty] public long CurrentPhaseRevision { get; }

        [JsonProperty] public GamePhaseType NextPhaseType { get; }

        [JsonProperty] public long PhaseRevision { get; }

        public override IActionValidator GetValidator()
        {
            return new Validator(CurrentPhaseType, CurrentPhaseRevision, NextPhaseType,
                PhaseRevision);
        }

        protected override IReadOnlyList<IGameEvent> ExecuteCore(GameContext context, GameActor actor)
        {
            return new IGameEvent[] { new PhaseChangedGameEvent(NextPhaseType, PhaseRevision) };
        }

        private sealed class Validator : IActionValidator
        {
            private readonly GamePhaseType _currentPhaseType;
            private readonly long _currentPhaseRevision;
            private readonly GamePhaseType _nextPhaseType;
            private readonly long _phaseRevision;

            public Validator(GamePhaseType currentPhaseType, long currentPhaseRevision,
                GamePhaseType nextPhaseType, long phaseRevision)
            {
                _currentPhaseType = currentPhaseType;
                _currentPhaseRevision = currentPhaseRevision;
                _nextPhaseType = nextPhaseType;
                _phaseRevision = phaseRevision;
            }

            public ValidationResult Check(GameContext context, GameActor actor)
            {
                if (!ValidateCurrentPhase(context.Phase, out var error))
                {
                    return ValidationResult.Failed((int)Status.StalePhase, error);
                }

                if (!ValidateNewRevision(context, out error))
                {
                    return ValidationResult.Failed((int)Status.InvalidRevision, error);
                }

                if (!ValidateTransition(context.Phase, out error))
                {
                    return ValidationResult.Failed((int)Status.UnsupportedTransition,
                        error);
                }

                if (!ValidateParticipants(context, out error))
                {
                    return ValidationResult.Failed((int)Status.MissingParticipants,
                        error);
                }

                if (!ValidateTransitionTime(context, out error))
                {
                    return ValidationResult.Failed((int)Status.TransitionNotDue, error);
                }

                return ValidationResult.Ok;
            }

            private static bool ValidateParticipants(GameContext context, out string error)
            {
                if (context.Players.All.Count == 0)
                {
                    error = "A phase transition requires participants.";
                    return false;
                }

                error = string.Empty;
                return true;
            }

            private static bool ValidateTransitionTime(GameContext context, out string error)
            {
                error = string.Empty;
                var phase = context.Phase;
                if (!TransitionPhaseHelper.IsTimedPhase(phase.Type))
                {
                    return true;
                }

                if (!phase.EndTimeInSeconds.HasValue)
                {
                    error = "The phase timer has not started.";
                    return false;
                }

                var now = context.Time.ServerTimeInSeconds;
                var endTime = phase.EndTimeInSeconds.Value;
                var readyTime = phase.ReadyTransitionTimeInSeconds;
                if (now < endTime && !(readyTime.HasValue && now >= readyTime.Value &&
                                       context.Players.All.All(p => p.IsReady)))
                {
                    error = "Neither the phase nor the ready deadline has elapsed.";
                    return false;
                }

                return true;
            }

            private bool ValidateCurrentPhase(PhaseContext phase, out string error)
            {
                if (phase.Type != _currentPhaseType || phase.Revision != _currentPhaseRevision)
                {
                    error = "The source phase or its revision is stale.";
                    return false;
                }

                error = string.Empty;
                return true;
            }

            private bool ValidateNewRevision(GameContext context, out string error)
            {
                if (context.Revision == long.MaxValue || _phaseRevision != context.Revision + 1 ||
                    _phaseRevision <= context.Phase.Revision)
                {
                    error = "The new phase revision must identify the next state commit.";
                    return false;
                }

                error = string.Empty;
                return true;
            }

            private bool ValidateTransition(PhaseContext phase, out string error)
            {
                if (!TransitionPhaseHelper.TryGetNextPhase(phase.Type, out var nextPhase) ||
                    nextPhase != _nextPhaseType)
                {
                    error = "The requested phase transition is not supported.";
                    return false;
                }

                error = string.Empty;
                return true;
            }
        }
    }
}