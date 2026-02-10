namespace Core.Game.Phases
{
    public sealed class GameInitializationPhase : BasePhase
    {
        private readonly IGalaxyManager _galaxyManager;
        private readonly ITwoPlayerFieldManager _fieldManager;
        
        public GameInitializationPhase(
            GameStateMachine stateMachine, 
            IGalaxyManager galaxyManager,
            ITwoPlayerFieldManager fieldManager) : base(stateMachine)
        {
            _galaxyManager = galaxyManager;
            _fieldManager = fieldManager;
        }

        public override void Enter()
        {
            _galaxyManager.Init();
            _fieldManager.Init();
        }

        public override void Accept(IPhaseVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}