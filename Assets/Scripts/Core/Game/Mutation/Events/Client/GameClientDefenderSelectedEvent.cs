using Core.Game.Encounter;
using Core.Game.Rules;
using Logs;

namespace Core.Game.Mutation.Events
{
    public sealed class GameClientDefenderSelectedEvent : IClientGameEvent
    {
        private readonly IGameClientEncounterEvents _encounterEvents;
        private readonly GameRulesChecker _rulesChecker;
        private readonly ulong _defenderPlayerId;
        
        public GameClientDefenderSelectedEvent(
            int eventId, 
            IGameClientEncounterEvents encounterEvents,
            GameRulesChecker rulesChecker,
            ulong defenderPlayerId)
        {
            EventId = eventId;
            _encounterEvents = encounterEvents;
            _rulesChecker = rulesChecker;
            _defenderPlayerId = defenderPlayerId;
        }
        
        public int EventId { get; }
        
        public void Apply()
        {
            var context = GameRuleContext.CheckPlayer(_defenderPlayerId);
            
            if (!_rulesChecker.Check(GameRuleType.CanBeDefender, context))
            {
                Logger.Error($"{nameof(GameClientDefenderSelectedEvent)}.{nameof(Apply)}: it is impossible to choose an defender.");
                return;
            }
            
            _encounterEvents.SetDefenderEvent(_defenderPlayerId);
        }
    }
}