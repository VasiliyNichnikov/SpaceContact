using System;

namespace Network.Dto
{
    [Serializable]
    public class GameAggressorSelectedEventStateData
    {
        public ulong AggressorPlayerId;
        
        public GameEventMetadata Metadata;
    }
}