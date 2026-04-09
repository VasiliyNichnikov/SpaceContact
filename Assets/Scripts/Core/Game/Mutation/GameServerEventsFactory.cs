using Core.Game.Dto.States.Cards;
using Core.Game.Mutation.Events;

namespace Core.Game.Mutation
{
    public sealed class GameServerEventsFactory
    {
        private int _lastEventId;
        
        public GameServerAggressorSelectedEvent CreateAggressorSelectedEvent(ulong aggressorId)
        {
            var eventId = GetCurrentEventId();
            var aggressorSelectedEvent = new GameServerAggressorSelectedEvent(eventId, aggressorId);

            return aggressorSelectedEvent;
        }

        public GameServerDefenderSelectedEvent CreateDefenderSelectedEvent(ulong defenderId)
        {
            var eventId = GetCurrentEventId();
            var defenderSelectedEvent = new GameServerDefenderSelectedEvent(eventId, defenderId);
            
            return defenderSelectedEvent;
        }

        public GameServerDestinyCardChangedEvent CreateDestinyCardChangedEvent(DestinyCardData destinyCardData)
        {
            var eventId = GetCurrentEventId();
            var destinyCardChangedEvent = new GameServerDestinyCardChangedEvent(eventId, destinyCardData);
            
            return destinyCardChangedEvent;
        }
        
        private int GetCurrentEventId()
        {
            var lastEventId = _lastEventId;
            lastEventId++;
            return lastEventId;
        }
    }
}