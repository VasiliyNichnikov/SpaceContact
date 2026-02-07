namespace Core.Phases
{
    public abstract class BasePhase : IGamePhase
    {
        protected readonly GameStateMachine StateMachine;
        
        protected BasePhase(GameStateMachine stateMachine)
        {
            StateMachine = stateMachine;
        }
        
        public virtual void Enter()
        {
            // nothing
        }

        public virtual void Exit()
        {
            // nothing
        }

        public virtual void Update()
        {
            // nothing
        }
        
        public abstract void Accept(IPhaseVisitor visitor);
    }
}