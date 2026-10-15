using System.Collections.Generic;
using System;
using Core.Game.Validation;
using Core.Game.Visibility;
using Newtonsoft.Json;

namespace Core.Game.Dto.Snapshots
{
    [Serializable]
    [StateRule(typeof(PlayerHandShapeRule))]
    [JsonObject(MemberSerialization.OptIn, ItemNullValueHandling = NullValueHandling.Ignore)]
    public sealed class PlayerHandSnapshot
    {
        [JsonProperty]
        [StateVisibility(StateAudience.Owner)]
        [StateRequired]
        public List<SpaceCardSnapshot>? Cards;

        private int? _declaredCount;

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
            _declaredCount = hiddenCardsCount;
        }

        [JsonProperty]
        [StateVisibility(StateAudience.Everyone)]
        [StateRequired]
        [StateRange(0)]
        public int Count
        {
            get => Cards?.Count ?? _declaredCount ?? 0;
            private set => _declaredCount = value;
        }

        [JsonIgnore]
        internal bool HasConsistentCount =>
            !_declaredCount.HasValue || Cards is null || _declaredCount.Value == Cards.Count;
    }
}
