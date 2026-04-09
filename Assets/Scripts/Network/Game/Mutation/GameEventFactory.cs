using Core.Game.Encounter;
using Core.Game.Mutation;
using Core.Game.Mutation.Events;
using Core.Game.Phases.Client;
using Core.Game.Rules;
using Logs;
using Network.Dto;

namespace Network.Game.Mutation
{
    public sealed class GameEventFactory
    {
        private readonly IGameClientEncounterEvents _encounterEvents;
        private readonly IGameClientDestinyPhaseResolver _destinyPhaseResolver;
        private readonly GameRulesChecker _rulesChecker;
        
        public GameEventFactory(
            IGameClientEncounterEvents encounterEvents,
            IGameClientDestinyPhaseResolver destinyPhaseResolver,
            GameRulesChecker rulesChecker)
        {
            _encounterEvents = encounterEvents;
            _destinyPhaseResolver = destinyPhaseResolver;
            _rulesChecker = rulesChecker;
        }
        
        public IClientGameEvent Create(GameEventStateData eventData)
        {
            if (eventData.HasAggressorSelectedEvent)
            {
                var aggressorEventData = eventData.AggressorSelectedEvent;
                
                return new GameClientAggressorSelectedEvent(
                    aggressorEventData.Metadata.EventId, 
                    _encounterEvents,
                    _rulesChecker,
                    aggressorEventData.AggressorPlayerId);
            }

            if (eventData.HasDefenderSelectedEvent)
            {
                var defenderEventData = eventData.DefenderSelectedEvent;
                
                return new GameClientDefenderSelectedEvent(
                    defenderEventData.Metadata.EventId,
                    _encounterEvents,
                    _rulesChecker,
                    defenderEventData.DefenderPlayerId);
            }

            if (eventData.HasDestinyCardChangedEvent)
            {
                var destinyCardChangedEventData = eventData.DestinyCardChangedEvent;

                return new GameClientDestinyCardChangedEvent(
                    destinyCardChangedEventData.Metadata.EventId,
                    _destinyPhaseResolver,
                    _rulesChecker,
                    destinyCardChangedEventData.DestinyCard);
            }

            Logger.Error($"{nameof(GameEventFactory)}.{nameof(Create)}: eventData is not support.");
            return GameClientErrorEvent.Instance;
        }
    }
}