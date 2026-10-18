using System;
using Newtonsoft.Json;

namespace Core.Game.Events
{
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class ApplyPhaseTransitionTimerCancellationGameEvent : PublicGameEvent
    {
        public override void Apply(GameContext context)
        {
            context.Phase.ResetReadyTransitionTime();
        }
    }
}
