using System;
using Core.Game.Dto.States.Cards;

namespace Network.Dto
{
    [Serializable]
    public struct GameDestinyCardChangedEventStateData
    {
        public DestinyCardData DestinyCard;
        
        public GameEventMetadata Metadata;
    }
}