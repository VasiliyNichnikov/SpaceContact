using System.Threading.Tasks;

namespace Core.Game.Phases
{
    public abstract class BasePhase : IGamePhase
    {
        protected readonly GameStateMachine StateMachine;
        
        protected BasePhase(GameStateMachine stateMachine)
        {
            StateMachine = stateMachine;
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