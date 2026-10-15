using System;
using Core.Game.Visibility;
using Newtonsoft.Json;

namespace Core.Game.Events
{
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class ApplyDestinyCardGameEvent : PublicGameEvent
    {
        public ApplyDestinyCardGameEvent(DestinyCardData destinyCard)
        {
            DestinyCard = destinyCard;
        }

        [JsonProperty]
        [StateVisibility(StateAudience.Everyone)]
        public DestinyCardData DestinyCard { get; }

        public override void Apply(GameContext context)
        {
            context.Cards.SetActiveDestinyCard(DestinyCard);
        }
    }
}
