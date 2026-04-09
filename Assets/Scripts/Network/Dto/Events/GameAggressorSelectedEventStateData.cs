using System;

namespace Network.Dto
{
    [Serializable]
    public struct GameAggressorSelectedEventStateData
    {
        public ulong AggressorPlayerId;
        
        public GameEventMetadata Metadata;
    }
}