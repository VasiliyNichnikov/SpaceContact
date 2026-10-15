using System;
using Core.Game.Visibility;
using Newtonsoft.Json;

namespace Core.Game.Events
{
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public class ApplyAggressorPlayerGameEvent : PublicGameEvent
    {
        public ApplyAggressorPlayerGameEvent(ulong aggressorPlayerId)
        {
            AggressorPlayerId = aggressorPlayerId;
        }

        [JsonProperty]
        [StateVisibility(StateAudience.Everyone)]
        public ulong AggressorPlayerId { get; }

        public override void Apply(GameContext context)
        {
            context.Encounter.SetAggressor(AggressorPlayerId);
        }
    }
}
