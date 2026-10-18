using System;
using Core.Game.Dto.Snapshots;
using Core.Game.Events;
using Core.Helpers;
using Newtonsoft.Json;

namespace Core.Game.Actions
{
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class StartPhaseTimerGameAction : BaseGameAction
    {
        private const double DeadlineToleranceInSeconds = 0.000001;

        public StartPhaseTimerGameAction(GamePhaseType phaseType, long phaseRevision, double durationInSeconds)
        {
            PhaseType = phaseType;
            PhaseRevision = phaseRevision;
            DurationInSeconds = durationInSeconds;
        }

        [JsonProperty]
        public GamePhaseType PhaseType { get; }

        [JsonProperty]
        public long PhaseRevision { get; }

        [JsonProperty]
        public double DurationInSeconds { get; }

        public override GameActionResult Execute(GameContext context, GameActor actor)
        {
            if (context.Role != GameContextRole.Server || !actor.IsSystem)
            {
                return GameActionResult.Failed("Only the system can start the phase timer on the server.");
            }

            if (context.Players.All.Count == 0)
            {
                return GameActionResult.Failed("A phase timer requires participants.");
            }

            var phase = context.Phase;
            if (phase.Type != PhaseType || phase.Revision != PhaseRevision || !TransitionPhaseHelper.IsTimedPhase(PhaseType))
            {
                return GameActionResult.Failed("The phase entry is stale or does not support a timer.");
            }

            if (phase.EndTimeInSeconds.HasValue || phase.ReadyTransitionTimeInSeconds.HasValue)
            {
                return GameActionResult.Failed("A timer has already started for this phase entry.");
            }

            var now = context.Time.ServerTimeInSeconds;
            var endTime = now + DurationInSeconds;
            if (DurationInSeconds > 0 && Math.Abs(endTime - now) < DeadlineToleranceInSeconds)
            {
                return GameActionResult.Failed("The phase deadline is too close to the current server time.");
            }

            return GameActionResult.Success(new PhaseTimerStartedGameEvent(PhaseRevision, endTime));
        }
    }
}
