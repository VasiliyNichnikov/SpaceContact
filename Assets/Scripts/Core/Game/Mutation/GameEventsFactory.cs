using Core.Game.Dto;
using Core.Game.Dto.States.Cards;

namespace Core.Game.Mutation
{
    public sealed class GameEventsFactory
    {
        private int _lastEventId;
        private readonly IGameServerTime _serverTime;

        public GameEventsFactory(IGameServerTime serverTime)
        {
            _serverTime = serverTime;
        }
        
        public GameDefenderSelectedEventData CreateDefenderSelectedEvent(ulong defenderId)
        {
            var eventId = GetCurrentEventId();
            var metadata = CreateMetadata(eventId);
            
            var defenderSelectedEvent = new GameDefenderSelectedEventData
            {
                DefenderPlayerId = defenderId,
                Metadata = metadata
            };
            
            return defenderSelectedEvent;
        }

        public GameDestinyCardChangedEventData CreateDestinyCardChangedEvent(DestinyCardData destinyCardData)
        {
            var eventId = GetCurrentEventId();
            var metadata = CreateMetadata(eventId);

            var destinyCardChangedEvent = new GameDestinyCardChangedEventData
            {
                DestinyCard = destinyCardData,
                Metadata = metadata
            };
            
            return destinyCardChangedEvent;
        }

        public GamePlanetToAttackSelectedEventData CreatePlanetIdToAttackSelectedEvent(ulong initiatedByPlayerId, int planetId)
        {
            var eventId = GetCurrentEventId();
            var metadata = CreateMetadata(eventId);

            var planetIdToAttackSelectedEvent = new GamePlanetToAttackSelectedEventData
            {
                InitiatedByPlayerId = initiatedByPlayerId,
                PlanetId = planetId,
                Metadata = metadata
            };

            return planetIdToAttackSelectedEvent;
        }

        private GameEventMetadata CreateMetadata(int eventId)
        {
            return new GameEventMetadata()
            {
                EventId = eventId,
                CreatedAtSeconds = _serverTime.ServerTimeInSeconds
            };
        }
        
        private int GetCurrentEventId()
        {
            var lastEventId = _lastEventId;
            _lastEventId++;
            return lastEventId;
        }
    }
}