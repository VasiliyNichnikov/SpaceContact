using System;
using Core.Game.Visibility;
using Newtonsoft.Json;

namespace Core.Game.Events
{
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class PhaseTimerStartedGameEvent : PublicGameEvent
    {
        public PhaseTimerStartedGameEvent(long phaseRevision, double endTimeInSeconds)
        {
            PhaseRevision = phaseRevision;
            EndTimeInSeconds = endTimeInSeconds;
        }

        [JsonProperty]
        [StateVisibility(StateAudience.Everyone)]
        public long PhaseRevision { get; }

        [JsonProperty]
        [StateVisibility(StateAudience.Everyone)]
        public double EndTimeInSeconds { get; }

        public override void Apply(GameContext context)
        {
            context.Phase.StartTimer(EndTimeInSeconds);
        }
    }
}
