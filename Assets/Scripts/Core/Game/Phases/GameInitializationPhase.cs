using Logs;

namespace Core.Game.Phases
{
    public sealed class GameInitializationPhase : BasePhase
    {
        private readonly IGameFieldManager _fieldManager;
        private readonly GamePlayersPhaseTracker _playersPhaseTracker;
        private readonly IServerStateMachineNetwork? _serverStateMachine;
        
        public GameInitializationPhase(
            GameStateMachine stateMachine, 
            IGameFieldManager fieldManager,
            GamePlayersPhaseTracker playersPhaseTracker,
            IServerStateMachineNetwork? serverStateMachine) : base(stateMachine)
        {
            _fieldManager = fieldManager;
            _playersPhaseTracker = playersPhaseTracker;
            _serverStateMachine = serverStateMachine;
            _playersPhaseTracker.PlayerPhaseChanged += OnPlayerPhaseChanged;
        }

        public override GamePhaseType Type => 
            GamePhaseType.Initialization;

        public override void Enter()
        {
            Logger.Warning("GameInitializationPhase.Enter");
            _fieldManager.Init();
        }

        public override void Exit()
        {
            _playersPhaseTracker.PlayerPhaseChanged -= OnPlayerPhaseChanged;
        }

        public override void Accept(IPhaseVisitor visitor) => 
            visitor.Visit(this);

        private void OnPlayerPhaseChanged(ulong playerId)
        {
            if (_serverStateMachine == null)
            {
                return;
            }

            if (!_playersPhaseTracker.AreAllPlayersInPhase(GamePhaseType.Initialization))
            {
                return;
            }
            
            _serverStateMachine.ServerTransitionTo<GameDestinyPhase>();
        }
    }
}