using System.Threading.Tasks;

namespace Core.Game.Phases
{
    public abstract class BasePhaseWithContext<TContext> : IGamePhaseWithContext<TContext> where TContext : IPhasePayload
    {
        private readonly GamePhaseTimeController _phaseTimeController;
        
        protected BasePhaseWithContext(GamePhaseTimeController phaseTimeController)
        {
            _phaseTimeController = phaseTimeController;
        }
        
        protected TContext? Context { get; private set; }
        
        public void SetContext(IPhasePayload context)
        {
            Context = (TContext)context;
            _phaseTimeController.SetEndTimeInSeconds(context.EndPhaseTime);
        }

        public abstract GamePhaseType Type { get; }
        
        public bool IsFinished => 
            _phaseTimeController.IsFinished;
        
        public int RemainingTime => 
            _phaseTimeController.RemainingTime;

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