using System;
using System.Collections.Generic;

namespace Core.Game.Dto.Snapshots
{
    [Serializable]
    public sealed class CardsSnapshot
    {
        // Null means hidden, while an empty list means a known, exhausted deck.
        public List<SpaceCardSnapshot>? SpaceDeck = new();

        public List<SpaceCardSnapshot> SpaceDiscard = new();
        
        public List<DestinyCardSnapshot>? DestinyDeck = new();

        public List<DestinyCardSnapshot> DestinyDiscard = new();

        public DestinyCardSnapshot? ActiveDestinyCard;

        public Dictionary<ulong, PlayerHandSnapshot> HandsByPlayerId = new();
    }
}
