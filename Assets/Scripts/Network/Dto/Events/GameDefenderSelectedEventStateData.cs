using System;

namespace Network.Dto
{
    [Serializable]
    public class GameDefenderSelectedEventStateData
    {
        public ulong DefenderPlayerId;
        
        public GameEventMetadata Metadata;
    }
}