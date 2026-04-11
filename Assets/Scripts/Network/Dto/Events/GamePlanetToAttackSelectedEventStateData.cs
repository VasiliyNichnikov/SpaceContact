using System;

namespace Network.Dto
{
    [Serializable]
    public struct GamePlanetToAttackSelectedEventStateData
    {
        public int PlanetId;
        
        public ulong InitiatedByPlayerId;
        
        public GameEventMetadata Metadata;
    }
}