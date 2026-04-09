using Core.Game.Encounter;
using Core.Game.Rules;
using Logs;

namespace Core.Game.Mutation.Events
{
    public sealed class GameClientAggressorSelectedEvent : IClientGameEvent
    {
        private readonly IGameClientEncounterEvents _encounterEvents;
        private readonly GameRulesChecker _rulesChecker;
        private readonly ulong _aggressorPlayerId;
        
        public GameClientAggressorSelectedEvent(
            int eventId, 
            IGameClientEncounterEvents encounterEvents,
            GameRulesChecker rulesChecker,
            ulong aggressorPlayerId)
        {
            EventId = eventId;
            _rulesChecker = rulesChecker;
            _encounterEvents = encounterEvents;
            _aggressorPlayerId = aggressorPlayerId;
        }
        
        public int EventId { get; }
        
        public void Apply()
        {
            var context = GameRuleContext.CheckPlayer(_aggressorPlayerId);
            
            if (!_rulesChecker.Check(GameRuleType.CanBeAggressor, context))
            {
                Logger.Error($"{nameof(GameClientAggressorSelectedEvent)}.{nameof(Apply)}: it is impossible to choose an aggressor.");
                return;
            }
            
            _encounterEvents.SetAggressorEvent(_aggressorPlayerId);
        }
    }
}