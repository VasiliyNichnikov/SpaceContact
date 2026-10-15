using System.Collections.Generic;
using System;
using Core.Game.Visibility;
using Newtonsoft.Json;

namespace Core.Game.Dto.Snapshots
{
    [Serializable]
    [JsonObject(MemberSerialization.OptIn, ItemNullValueHandling = NullValueHandling.Ignore)]
    public sealed class PlayerHandSnapshot
    {
        [JsonProperty]
        [StateVisibility(StateAudience.Owner)]
        public List<SpaceCardSnapshot>? Cards;

        private int _hiddenCardsCount;

        [JsonConstructor]
        public PlayerHandSnapshot()
        {
        }

        public PlayerHandSnapshot(List<SpaceCardSnapshot> cards)
        {
            Cards = cards;
        }

        public PlayerHandSnapshot(int hiddenCardsCount)
        {
            _hiddenCardsCount = hiddenCardsCount;
        }

        [JsonProperty]
        [StateVisibility(StateAudience.Everyone)]
        public int Count
        {
            get => Cards?.Count ?? _hiddenCardsCount;
            private set => _hiddenCardsCount = value;
        }
    }
}
