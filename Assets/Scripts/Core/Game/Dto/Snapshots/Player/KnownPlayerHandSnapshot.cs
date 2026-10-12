using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Core.Game.Dto.Snapshots
{
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class KnownPlayerHandSnapshot : PlayerHandSnapshot
    {
        [JsonProperty]
        public List<SpaceCardSnapshot> Cards;

        [JsonConstructor]
        public KnownPlayerHandSnapshot(List<SpaceCardSnapshot> cards)
        {
            Cards = cards;
        }

        public KnownPlayerHandSnapshot() : this(new List<SpaceCardSnapshot>())
        {
        }

        [JsonIgnore]
        public override int Count => Cards.Count;
    }
}