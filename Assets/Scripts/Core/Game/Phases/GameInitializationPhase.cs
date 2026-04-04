using Logs;

namespace Core.Game.Phases
{
    public sealed class GameInitializationPhase : BasePhase
    {
        private readonly GamePlayersPhaseTracker _playersPhaseTracker;
        private readonly IServerStateMachineNetwork? _serverStateMachine;
        
        public GameInitializationPhase(
            GameStateMachine stateMachine, 
            GamePlayersPhaseTracker playersPhaseTracker,
            IServerStateMachineNetwork? serverStateMachine) : base(stateMachine)
        {
            _playersPhaseTracker = playersPhaseTracker;
            _serverStateMachine = serverStateMachine;
            _playersPhaseTracker.PlayerPhaseChanged += OnPlayerPhaseChanged;
        }

        public override GamePhaseType Type => 
            GamePhaseType.Initialization;

        public override void Enter()
        {
            Logger.Warning("GameInitializationPhase.Enter");
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