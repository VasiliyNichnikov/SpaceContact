using System.Threading.Tasks;

namespace Core.Game.Phases
{
    public abstract class BasePhaseWithContext<TContext> : IGamePhaseWithContext<TContext> where TContext : IPhasePayload
    {
        protected readonly GameStateMachine StateMachine;
        
        protected BasePhaseWithContext(GameStateMachine stateMachine)
        {
            StateMachine = stateMachine;
        }
        
        protected TContext? Context { get; private set; }
        
        public void SetContext(IPhasePayload context)
        {
            Context = (TContext)context;
        }

        public abstract GamePhaseType Type { get; }
        
        public virtual Task Enter() => 
            Task.CompletedTask;

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