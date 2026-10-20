using System;
using Core.Game.Visibility;
using Newtonsoft.Json;

namespace Core.Game.Events
{
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class ApplyDestinyDeckCountGameEvent : PublicGameEvent
    {
        public ApplyDestinyDeckCountGameEvent(int destinyDeckCount)
        {
            DestinyDeckCount = destinyDeckCount;
        }

        [JsonProperty(Required = Required.Always)]
        [StateVisibility(StateAudience.Everyone)]
        public int DestinyDeckCount { get; }

        public override void Apply(GameContext context)
        {
            context.Cards.SetDestinyDeckCount(DestinyDeckCount);
        }
    }
}
