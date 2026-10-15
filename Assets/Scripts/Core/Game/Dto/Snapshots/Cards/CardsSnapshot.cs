using System.Collections.Generic;
using System;
using Core.Game.Validation;
using Core.Game.Visibility;
using Newtonsoft.Json;

namespace Core.Game.Dto.Snapshots
{
    [Serializable]
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
