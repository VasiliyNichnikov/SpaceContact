using System;
using System.Linq;
using Core.Game.Events;
using Core.Helpers;
using Newtonsoft.Json;

namespace Core.Game.Actions
{
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class CancelReadyTransitionTimerGameAction : BaseGameAction
    {
        public CancelReadyTransitionTimerGameAction(long phaseRevision)
        {
            PhaseRevision = phaseRevision;
        }

        [JsonProperty]
        public long PhaseRevision { get; }

        public override GameActionResult Execute(GameContext context, GameActor actor)
        {
            if (context.Role != GameContextRole.Server || !actor.IsSystem)
            {
                return GameActionResult.Failed("Only the system can cancel the ready timer on the server.");
            }

            var phase = context.Phase;
            
            if (phase.Revision != PhaseRevision || !TransitionPhaseHelper.IsTimedPhase(phase.Type))
            {
                return GameActionResult.Failed("The phase entry is stale or does not support a ready timer.");
            }

            if (!phase.EndTimeInSeconds.HasValue || !phase.ReadyTransitionTimeInSeconds.HasValue)
            {
                return GameActionResult.Failed("The phase and ready timers must be running.");
            }

            var now = context.Time.ServerTimeInSeconds;
            var endTime = phase.EndTimeInSeconds.Value;

            if (now >= endTime)
            {
                return GameActionResult.Failed("The phase deadline has elapsed and cannot be cancelled.");
            }

            if (context.Players.All.Count > 0 && context.Players.All.All(player => player.IsReady))
            {
                return GameActionResult.Failed("The ready timer cannot be cancelled while all participants are ready.");
            }

            return GameActionResult.Success(new ApplyPhaseTransitionTimerCancellationGameEvent());
        }
    }
}
