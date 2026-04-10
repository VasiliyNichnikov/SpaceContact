using Core.Game.Phases;

namespace Core.Game.Rules
{
    public class GameCanApplyDestinyCardRule : IGameRule
    {
        private readonly IGameStateMachineReadOnly _stateMachine;
        
        public GameCanApplyDestinyCardRule(IGameStateMachineReadOnly stateMachine)
        {
            _stateMachine = stateMachine;
        }
        
        public GameRuleType Type => 
            GameRuleType.CanApplyDestinyCard;
        
        public bool Check(GameRuleContext context)
        {
            return _stateMachine.CurrentPhase is { Type: GamePhaseType.Destiny };
        }
    }
}