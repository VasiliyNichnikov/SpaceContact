using System;
using Core.Game.Visibility;
using Newtonsoft.Json;

namespace Core.Game.Events
{
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class ApplyStartReadyTransitionTimerGameEvent : PublicGameEvent
    {
        public ApplyStartReadyTransitionTimerGameEvent(double readyTransitionTimeInSeconds)
        {
            ReadyTransitionTimeInSeconds = readyTransitionTimeInSeconds;
        }

        [JsonProperty]
        [StateVisibility(StateAudience.Everyone)]
        public double ReadyTransitionTimeInSeconds { get; }

        public override void Apply(GameContext context)
        {
            context.Phase.SetReadyTransitionTime(ReadyTransitionTimeInSeconds);
        }
    }
}
