using System.Threading;
using System.Threading.Tasks;
using Core.Game.Cards;
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
        private readonly IGameClientPlayerCardsDeckService _clientPlayerCardsDeckService;
        
        private readonly GameServerPhaseTransitioner? _transitioner;

        private readonly CancellationTokenSource _cts = new();
        
        public GameInitializationPhase(
            GamePlayersPhaseTracker playersPhaseTracker,
            IGameClientGalaxyManager clientGalaxyManager,
            IGameClientPlayerCardsDeckService clientPlayerCardsDeckService,
            IGamePhaseServerInteraction serverInteraction,
            
            GameServerPhaseTransitioner? transitioner)
        {
            _playersPhaseTracker = playersPhaseTracker;
            _clientGalaxyManager = clientGalaxyManager;
            _clientPlayerCardsDeckService = clientPlayerCardsDeckService;
            _serverInteraction = serverInteraction;
            _transitioner = transitioner;
        }

        public override GamePhaseType Type => 
            GamePhaseType.Initialization;

        public override Task Enter()
        {
            Logger.Warning($"{nameof(GameInitializationPhase)}.{nameof(Enter)}");
            
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
            
            if (galaxyState == null || _cts.Token.IsCancellationRequested)
            {
                return;
            }

            await _clientPlayerCardsDeckService.InitPlayersHands(_cts.Token);

            if (_cts.Token.IsCancellationRequested)
            {
                return;
            }
            
            _clientGalaxyManager.UpdateState(galaxyState);
            OnInitialized();
        }

        private void OnInitialized()
        {
            if (_transitioner == null)
            {
                return;
            }

            if (!_playersPhaseTracker.AreAllPlayersInPhase(GamePhaseType.Initialization))
            {
                return;
            }
            
            _transitioner.OnInitialized();
        }
    }
}