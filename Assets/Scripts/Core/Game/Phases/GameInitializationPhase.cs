using Core.Game.Dto.Payload;
using Core.Game.Encounter;
using Core.Game.Phases.Server;
using Logs;

namespace Core.Game.Phases
{
    public sealed class GameInitializationPhase : BasePhase
    {
        private readonly GamePlayersPhaseTracker _playersPhaseTracker;
        private readonly IServerStateMachineNetwork? _serverStateMachine;
        private readonly IGameServerDestinyPhaseResolver? _serverDestinyPhaseResolver;
        private readonly IGameServerEncounterManager? _serverEncounterManager;
        
        public GameInitializationPhase(
            GameStateMachine stateMachine, 
            GamePlayersPhaseTracker playersPhaseTracker,
            IGameServerEncounterManager? serverEncounterManager,
            IGameServerDestinyPhaseResolver? serverDestinyPhaseResolver,
            IServerStateMachineNetwork? serverStateMachine) : base(stateMachine)
        {
            _playersPhaseTracker = playersPhaseTracker;
            _serverStateMachine = serverStateMachine;
            _serverDestinyPhaseResolver = serverDestinyPhaseResolver;
            _serverEncounterManager = serverEncounterManager;
        }

        public override GamePhaseType Type => 
            GamePhaseType.Initialization;

        public override void Enter()
        {
            Logger.Warning("GameInitializationPhase.Enter");
            _serverEncounterManager?.StartEncounter(); 
            
            GoToDestinyPhase();
        }

        public override void Accept(IPhaseVisitor visitor) => 
            visitor.Visit(this);

        private void GoToDestinyPhase()
        {
            if (_serverStateMachine == null)
            {
                return;
            }

            if (!_playersPhaseTracker.AreAllPlayersInPhase(GamePhaseType.Initialization))
            {
                return;
            }

            if (_serverEncounterManager == null || _serverDestinyPhaseResolver == null)
            {
                Logger.Error($"{nameof(GameInitializationPhase)}.{nameof(GoToDestinyPhase)}: server data not found.");
                return;
            }
            
            var destinyPayload = new GamePhaseDestinyPayload
            {
                EncounterState = _serverEncounterManager.ToState()
            };
            
            _serverStateMachine.ServerTransitionTo<GameDestinyPhase>(destinyPayload);
        }
    }
}