using System;

namespace Network.Dto
{
    [Serializable]
    public struct GameEventStateData
    {
        public bool HasDefenderSelectedEvent;
        
        public GameDefenderSelectedEventStateData DefenderSelectedEvent;
        
        public bool HasAggressorSelectedEvent;
        
        public GameAggressorSelectedEventStateData AggressorSelectedEvent;
        
        public bool HasDestinyCardChangedEvent;
        
        public GameDestinyCardChangedEventStateData DestinyCardChangedEvent;

        public bool HasPlanetToAttackSelectedEvent;
        
        public GamePlanetToAttackSelectedEventStateData PlanetSelectedEvent;
    }
}