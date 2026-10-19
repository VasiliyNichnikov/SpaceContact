using System;
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
        public override GameActionResult Execute(GameContext context, GameActor actor)
        {
            if (context.Role != GameContextRole.Server || actor.IsSystem || !actor.PlayerId.HasValue)
            {
                return GameActionResult.Failed("Only a participant can change their readiness on the server.");
            }

            var playerId = actor.PlayerId.Value;
            if (!context.Players.IsPlayerExist(playerId))
            {
                return GameActionResult.Failed($"Player with id {playerId} not found.");
            }

            var phase = context.Phase;
            if (!TransitionPhaseHelper.IsTimedPhase(phase.Type) || !phase.EndTimeInSeconds.HasValue)
            {
                return GameActionResult.Failed("Readiness requires a running phase timer.");
            }

            var now = context.Time.ServerTimeInSeconds;
            var endTime = phase.EndTimeInSeconds.Value;
            var readyTime = phase.ReadyTransitionTimeInSeconds;

            if (now >= endTime)
            {
                return GameActionResult.Failed("The phase deadline has elapsed and cannot be cancelled.");
            }

            if (readyTime.HasValue && now >= readyTime.Value && context.Players.All.All(player => player.IsReady))
            {
                return GameActionResult.Failed("The ready deadline has elapsed.");
            }

            var wasReady = context.Players.Get(playerId).IsReady;
            var readyEvent = new PlayerReadyToggledGameEvent(playerId, !wasReady);

            if (wasReady && readyTime.HasValue)
            {
                return GameActionResult.Success(new IGameEvent[]
                {
                    readyEvent,
                    new ApplyPhaseTransitionTimerCancellationGameEvent()
                });
            }

            return GameActionResult.Success(readyEvent);
        }
    }
}
