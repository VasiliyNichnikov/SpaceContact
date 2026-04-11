using Core.Game.Encounter;
using Core.Game.Rules;
using Logs;

namespace Core.Game.Mutation.Events
{
    public class GameClientPlanetToAttackSelectedEvent : IClientGameEvent
    {
        private readonly IGameClientEncounterEvents _encounterEvents;
        private readonly GameRulesChecker _rulesChecker;
        private readonly int _planetIdToAttack;
        private readonly ulong _initiatedByPlayerId;
        
        public GameClientPlanetToAttackSelectedEvent(
            int eventId,
            IGameClientEncounterEvents encounterEvents,
            GameRulesChecker rulesChecker,
            int planetIdToAttack,
            ulong initiatedByPlayerId)
        {
            EventId = eventId;
            _encounterEvents = encounterEvents;
            _rulesChecker = rulesChecker;
            _planetIdToAttack = planetIdToAttack;
            _initiatedByPlayerId = initiatedByPlayerId;
        }
        
        public int EventId { get; }
        
        public void Apply()
        {
            var context = GameRuleContext.CheckPlanetToAttack(_initiatedByPlayerId, _planetIdToAttack);
            
            if (!_rulesChecker.Check(GameRuleType.CanChoosePlanetToAttack, context))
            {
                Logger.Error($"{nameof(GameClientPlanetToAttackSelectedEvent)}.{nameof(Apply)}: it is impossible to choose a planet to attack.");
                
                return;
            }
            
            _encounterEvents.SetPlanetIdToAttack(_planetIdToAttack);
        }
    }
}