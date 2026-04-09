using System;

namespace Network.Dto
{
    [Serializable]
    public struct GameDefenderSelectedEventStateData
    {
        public ulong DefenderPlayerId;
        
        public GameEventMetadata Metadata;
    }
}