using System;

namespace Network.Dto
{
    [Serializable]
    public class GameEventStateData
    {
        public GameDefenderSelectedEventStateData? DefenderSelectedEvent;
        
        public GameAggressorSelectedEventStateData? AggressorSelectedEvent;
    }
}