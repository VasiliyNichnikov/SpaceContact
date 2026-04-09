using System;

namespace Network.Dto
{
    [Serializable]
    public class GameEventsToClientsData
    {
        public GameEventStateData[] GameEvents = Array.Empty<GameEventStateData>();
    }
}