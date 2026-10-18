using System;
using System.Linq;
using Core.Game.Events;
using Core.Helpers;
using Newtonsoft.Json;

namespace Core.Game.Actions
{
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class StartReadyTransitionTimerGameAction : BaseGameAction
    {
        private const double DeadlineToleranceInSeconds = 0.000001;

        public StartReadyTransitionTimerGameAction(long phaseRevision)
        {
            PhaseRevision = phaseRevision;
        }

        [JsonProperty]
        public long PhaseRevision { get; }

        public override GameActionResult Execute(GameContext context, GameActor actor)
        {
            if (context.Role != GameContextRole.Server || !actor.IsSystem)
            {
                return GameActionResult.Failed("Only the system can start the ready timer on the server.");
            }

            var phase = context.Phase;
            if (phase.Revision != PhaseRevision || !TransitionPhaseHelper.IsTimedPhase(phase.Type))
            {
                return GameActionResult.Failed("The phase entry is stale or does not support a ready timer.");
            }

            if (!phase.EndTimeInSeconds.HasValue)
            {
                return GameActionResult.Failed("The phase timer has not started.");
            }

            if (context.Players.All.Count == 0 || !context.Players.All.All(player => player.IsReady))
            {
                return GameActionResult.Failed("The ready timer requires all participants to be ready.");
            }

            if (phase.ReadyTransitionTimeInSeconds.HasValue)
            {
                return GameActionResult.Failed("The ready timer is already running.");
            }

            var now = context.Time.ServerTimeInSeconds;
            var endTime = phase.EndTimeInSeconds.Value;

            if (now >= endTime)
            {
                return GameActionResult.Failed("The phase deadline has elapsed.");
            }

            var durationInSeconds = context.PhaseDurations.ReadyTransitionDurationInSeconds;
            var readyTime = now + durationInSeconds;

            return Math.Abs(readyTime - now - durationInSeconds) > DeadlineToleranceInSeconds
                ? GameActionResult.Failed("The ready deadline cannot represent the required duration.")
                : GameActionResult.Success(new ApplyStartReadyTransitionTimerGameEvent(readyTime));
        }
    }
}
