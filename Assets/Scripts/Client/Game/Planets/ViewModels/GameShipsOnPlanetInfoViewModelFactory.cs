using Client.UI.HUDs;
using Core.Game;
using Core.Game.Actions;

namespace Client.Game.Planets.ViewModels
{
    public sealed class GameShipsOnPlanetInfoViewModelFactory
    {
        private readonly IGameCurrentPlayerInfoTabController _infoTabController;
        private readonly GameContext _context;
        private readonly GameActionClient _actionClient;
        private readonly PendingGameActions _pendingActions;

        public GameShipsOnPlanetInfoViewModelFactory(IGameCurrentPlayerInfoTabController infoTabController,
            GameContext context, GameActionClient actionClient, PendingGameActions pendingActions)
        {
            _infoTabController = infoTabController;
            _context = context;
            _actionClient = actionClient;
            _pendingActions = pendingActions;
        }

        public GameShipsOnPlanetInfoViewModel Create(int planetId)
        {
            var viewModel = new GameShipsOnPlanetInfoViewModel(
                planetId, 
                _context, 
                _infoTabController,
                _actionClient, 
                _pendingActions);
            viewModel.Init();
            return viewModel;
        }
    }
}
