using Core.Game.Phases;
using Core.Game.Phases.Client;

namespace Core.Game.Rules
{
    public class GameCanApplyDestinyCardRule : IGameRule
    {
        private readonly IGameStateMachineReadOnly _stateMachine;
        private readonly IGameClientDestinyPhaseResolver _destinyPhaseResolver;
        
        public GameCanApplyDestinyCardRule(
            IGameStateMachineReadOnly stateMachine,
            IGameClientDestinyPhaseResolver destinyPhaseResolver)
        {
            _stateMachine = stateMachine;
            _destinyPhaseResolver = destinyPhaseResolver;
        }
        
        public GameRuleType Type => 
            GameRuleType.CanApplyDestinyCard;
        
        public bool Check(GameRuleContext context)
        {
            if (_stateMachine.CurrentPhase is not { Type: GamePhaseType.Destiny })
            {
                return false;
            }

            return _destinyPhaseResolver.Card == null;
        }
    }
}