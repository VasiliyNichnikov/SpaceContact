using System.Threading;
using System.Threading.Tasks;
using Core.Game.Dto.Payload;
using Core.Game.Encounter;
using Core.Game.Galaxy;
using Core.Game.Phases.Server;
using Logs;

namespace Core.Game.Phases
{
    public sealed class GameInitializationPhase : BasePhase
    {
        private readonly GamePlayersPhaseTracker _playersPhaseTracker;
        private readonly IGamePhaseServerInteraction _serverInteraction;
        private readonly IGameClientGalaxyManager _clientGalaxyManager;
        
        private readonly IServerStateMachineNetwork? _serverStateMachine;
        private readonly IGameServerDestinyPhaseResolver? _serverDestinyPhaseResolver;
        private readonly IGameServerEncounterManager? _serverEncounterManager;

        private readonly CancellationTokenSource _cts = new();
        
        public GameInitializationPhase(
            GameStateMachine stateMachine, 
            GamePlayersPhaseTracker playersPhaseTracker,
            IGameClientGalaxyManager clientGalaxyManager,
            IGamePhaseServerInteraction serverInteraction,
            
            IGameServerEncounterManager? serverEncounterManager,
            IGameServerDestinyPhaseResolver? serverDestinyPhaseResolver,
            IServerStateMachineNetwork? serverStateMachine) : base(stateMachine)
        {
            _playersPhaseTracker = playersPhaseTracker;
            _clientGalaxyManager = clientGalaxyManager;
            _serverInteraction = serverInteraction;
            _serverStateMachine = serverStateMachine;
            _serverDestinyPhaseResolver = serverDestinyPhaseResolver;
            _serverEncounterManager = serverEncounterManager;
        }

        public override GamePhaseType Type => 
            GamePhaseType.Initialization;

        public override Task Enter()
        {
            Logger.Warning("GameInitializationPhase.Enter");
            _serverEncounterManager?.StartEncounter();
            return LoadData();
        }

        public override void Exit()
        {
            _cts.Cancel();
            _cts.Dispose();
        }

        public override void Accept(IPhaseVisitor visitor) => 
            visitor.Visit(this);

        private async Task LoadData()
        {
            var galaxyState = await _serverInteraction.GetGalaxyStateAsync(_cts.Token);
            
            if (galaxyState == null || _cts.IsCancellationRequested)
            {
                return;
            }
            
            _clientGalaxyManager.UpdateState(galaxyState);
            
            GoToDestinyPhase();
        }

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