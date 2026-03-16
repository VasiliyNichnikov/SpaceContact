using System;
using System.Collections.Generic;

namespace Core.Game.Dto.States.Cards
{
    [Serializable]
    public class PlayerHandStateData
    {
        public int NumberOfCards;
        
        public List<SpaceCardStateData>? SpaceCardsOnYourHand;
    }
}