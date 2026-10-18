using System;
using Core.Game.Dto.Snapshots;
using Core.Game.Visibility;
using Newtonsoft.Json;

namespace Core.Game.Events
{
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class ApplyDestinyCardGameEvent : PublicGameEvent
    {
        public ApplyDestinyCardGameEvent(DestinyCardSnapshot destinyCard, ulong? defenderPlayerId)
        {
            DestinyCard = destinyCard;
            DefenderPlayerId = defenderPlayerId;
        }

        [JsonProperty]
        [StateVisibility(StateAudience.Everyone)]
        public DestinyCardSnapshot DestinyCard { get; }

        [JsonProperty]
        [StateVisibility(StateAudience.Everyone)]
        public ulong? DefenderPlayerId { get; }

        public override void Apply(GameContext context)
        {
            context.Encounter.ResetTarget();
            if (DefenderPlayerId.HasValue)
            {
                context.Encounter.SetDefender(DefenderPlayerId.Value);
            }

            context.Cards.SetActiveDestinyCard(DestinyCardData.Convert(DestinyCard));
        }
    }
}
