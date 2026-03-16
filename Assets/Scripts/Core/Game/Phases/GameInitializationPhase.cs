using Logs;

namespace Core.Game.Phases
{
    public sealed class GameInitializationPhase : BasePhase
    {
        private readonly IGameFieldManager _fieldManager;
        
        public GameInitializationPhase(
            GameStateMachine stateMachine, 
            IGameFieldManager fieldManager) : base(stateMachine)
        {
            _fieldManager = fieldManager;
        }

        public override void Enter()
        {
            Logger.Warning("GameInitializationPhase.Enter");
            _fieldManager.Init();
        }

        public override void Accept(IPhaseVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}