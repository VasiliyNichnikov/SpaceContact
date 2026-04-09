using Core.Game.Dto.States.Cards;
using Core.Game.Phases.Client;
using Core.Game.Rules;
using Logs;

namespace Core.Game.Mutation.Events
{
    public sealed class GameClientDestinyCardChangedEvent : IClientGameEvent
    {
        private readonly IGameClientDestinyPhaseResolver _destinyPhaseResolver;
        private readonly GameRulesChecker _rulesChecker;
        private readonly DestinyCardData _destinyCardData;
        
        public GameClientDestinyCardChangedEvent(
            int eventId,
            IGameClientDestinyPhaseResolver destinyPhaseResolver,
            GameRulesChecker rulesChecker,
            DestinyCardData destinyCardData)
        {
            EventId = eventId;
            _destinyPhaseResolver = destinyPhaseResolver;
            _rulesChecker = rulesChecker;
            _destinyCardData = destinyCardData;
        }
        
        public int EventId { get; }
        
        public void Apply()
        {
            if (!_rulesChecker.Check(GameRuleType.CanApplyDestinyCard, GameRuleContext.Empty))
            {
                Logger.Error($"{nameof(GameClientDestinyCardChangedEvent)}.{nameof(Apply)}: it is impossible to apply the destiny card.");
                
                return;
            }
            
            _destinyPhaseResolver.UpdateState(_destinyCardData);
        }
    }
}