using System;
using Newtonsoft.Json;

namespace Core.Game.Dto.Snapshots
{
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class HiddenPlayerHandSnapshot : PlayerHandSnapshot
    {
        [JsonProperty]
        public int CardsCount;

        public HiddenPlayerHandSnapshot(int cardsCount)
        {
            CardsCount = cardsCount;
        }

        [JsonIgnore]
        public override int Count => CardsCount;
    }
}