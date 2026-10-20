using System.Collections.Generic;
using System;
using Core.Game.Validation;
using Core.Game.Visibility;
using Newtonsoft.Json;

namespace Core.Game.Dto.Snapshots
{
    [Serializable]
    [StateRule(typeof(DestinyDeckRule))]
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public sealed class CardsSnapshot
    {
        // Null means hidden, while an empty list means a known, exhausted deck.
        [StateVisibility(StateAudience.ServerOnly)]
        [StateRequired]
        public List<SpaceCardSnapshot>? SpaceDeck;

        [StateVisibility(StateAudience.Everyone, Unordered = true)]
        [StateRequired]
        public List<SpaceCardSnapshot> SpaceDiscard = new();

        [StateVisibility(StateAudience.ServerOnly)]
        [StateRequired]
        public List<DestinyCardSnapshot>? DestinyDeck;

        [JsonProperty(Required = Required.Always)]
        [StateVisibility(StateAudience.Everyone)]
        [StateRequired]
        [StateRange(0, int.MaxValue)]
        public int DestinyDeckCount;

        [StateVisibility(StateAudience.Everyone, Unordered = true)]
        [StateRequired]
        public List<DestinyCardSnapshot> DestinyDiscard = new();

        [StateVisibility(StateAudience.Everyone)]
        public DestinyCardSnapshot? ActiveDestinyCard;

        [StateVisibility(StateAudience.Everyone)]
        [StateRequired]
        [StateReference(typeof(GamePlayerSnapshot), MatchAllIdentities = true)]
        public Dictionary<ulong, PlayerHandSnapshot> HandsByPlayerId = new();
    }
}
