using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Core.Game.Dto.Snapshots
{
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class PlayerHandSnapshot
    {
        [JsonIgnore]
        public abstract int Count { get; }
    }

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
            // nothing
        }
        
        [JsonIgnore]
        public override int Count => Cards.Count;
    }

    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class HiddenPlayerHandSnapshot : PlayerHandSnapshot
    {
        [JsonProperty]
        public int CardsCount;
        
        [JsonIgnore]
        public override int Count => CardsCount;
        
        public HiddenPlayerHandSnapshot(int cardsCount)
        {
            CardsCount = cardsCount;
        }
    }
}