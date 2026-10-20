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
    public sealed class TogglePlayerReadyGameAction : ClientGameAction
    {
        public enum Status
        {
            MissingParticipant = 0,
            UnknownParticipant = 1,
            PhaseTimerNotRunning = 2,
            PhaseDeadlineElapsed = 3,
            ReadyDeadlineElapsed = 4
        }
        
        public override IActionValidator GetValidator()
        {
            return new Validator();
        }

        protected override IReadOnlyList<IGameEvent> ExecuteCore(GameContext context, GameActor actor)
        {
            var playerId = actor.PlayerId
                ?? throw new InvalidOperationException("The validated ready action requires a participant.");
            var wasReady = context.Players.Get(playerId).IsReady;
            var readyEvent = new PlayerReadyToggledGameEvent(playerId, !wasReady);
            if (wasReady && context.Phase.ReadyTransitionTimeInSeconds.HasValue)
            {
                return new IGameEvent[] { readyEvent, new ApplyPhaseTransitionTimerCancellationGameEvent() };
            }

            return new IGameEvent[] { readyEvent };
        }
        
        private sealed class Validator : IActionValidator
        {
            public ValidationResult Check(GameContext context, GameActor actor)
            {
                if (!actor.PlayerId.HasValue)
                {
                    return ValidationResult.Failed((int)Status.MissingParticipant,
                        "Only a participant can change their readiness.");
                }

                var playerId = actor.PlayerId.Value;
                if (!context.Players.IsPlayerExist(playerId))
                {
                    return ValidationResult.Failed((int)Status.UnknownParticipant,
                        $"Player with id {playerId} not found.");
                }

                var phase = context.Phase;
                if (!TransitionPhaseHelper.IsTimedPhase(phase.Type) || !phase.EndTimeInSeconds.HasValue)
                {
                    return ValidationResult.Failed((int)Status.PhaseTimerNotRunning,
                        "Readiness requires a running phase timer.");
                }

                var now = context.Time.ServerTimeInSeconds;
                var endTime = phase.EndTimeInSeconds.Value;
                var readyTime = phase.ReadyTransitionTimeInSeconds;

                if (now >= endTime)
                {
                    return ValidationResult.Failed((int)Status.PhaseDeadlineElapsed,
                        "The phase deadline has elapsed and cannot be cancelled.");
                }

                if (readyTime.HasValue && now >= readyTime.Value && context.Players.All.All(player => player.IsReady))
                {
                    return ValidationResult.Failed((int)Status.ReadyDeadlineElapsed,
                        "The ready deadline has elapsed.");
                }

                return ValidationResult.Ok;
            }
        }
    }
}
