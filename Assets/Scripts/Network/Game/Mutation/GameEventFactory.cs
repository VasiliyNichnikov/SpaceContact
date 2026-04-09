using Core.Game.Encounter;
using Core.Game.Mutation;
using Core.Game.Mutation.Events;
using Core.Game.Rules;
using Logs;
using Network.Dto;

namespace Network.Game.Mutation
{
    public sealed class GameEventFactory
    {
        private readonly IGameClientEncounterEvents _encounterEvents;
        private readonly GameRulesChecker _rulesChecker;
        
        public GameEventFactory(
            IGameClientEncounterEvents encounterEvents,
            GameRulesChecker rulesChecker)
        {
            _encounterEvents = encounterEvents;
            _rulesChecker = rulesChecker;
        }
        
        public IClientGameEvent Create(GameEventStateData eventData)
        {
            if (eventData.AggressorSelectedEvent != null)
            {
                var aggressorEventData = eventData.AggressorSelectedEvent;
                
                return new GameClientAggressorSelectedEvent(
                    aggressorEventData.Metadata.EventId, 
                    _encounterEvents,
                    _rulesChecker,
                    aggressorEventData.AggressorPlayerId);
            }

            if (eventData.DefenderSelectedEvent != null)
            {
                var defenderEventData = eventData.DefenderSelectedEvent;
                
                return new GameClientDefenderSelectedEvent(
                    defenderEventData.Metadata.EventId,
                    _encounterEvents,
                    _rulesChecker,
                    defenderEventData.DefenderPlayerId);
            }

            Logger.Error($"{nameof(GameEventFactory)}.{nameof(Create)}: eventData is not support.");
            return GameClientErrorEvent.Instance;
        }
    }
}