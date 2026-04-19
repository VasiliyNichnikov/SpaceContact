using System;
using Core.Game.Mutation;

namespace Network.Dto
{
    [Serializable]
    public class GameEventsToClientsData
    {
        public IGameEventData[] GameEvents = Array.Empty<IGameEventData>();
    }
}