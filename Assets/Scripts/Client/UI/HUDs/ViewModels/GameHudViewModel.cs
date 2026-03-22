using Client.UI.Dialogs.Game.Hand.ViewModels;
using Core.Game.Hands;
using Core.Game.Players;

namespace Client.UI.HUDs.ViewModels
{
    public class GameHudViewModel : IGameHudViewModel
    {
        private readonly GamePlayersRegistry _registry;
        
        public GameHudViewModel(
            GamePlayersRegistry registry, 
            IGameHudTopViewModel topViewModel)
        {
            _registry = registry;
            TopViewModel = topViewModel;
            PlayerHandViewModel = CreatePlayerHandViewModel();
        }

        public IGameHudTopViewModel TopViewModel { get; }
        
        public IGamePlayerHandViewModel PlayerHandViewModel { get; }

        private IGamePlayerHandViewModel CreatePlayerHandViewModel()
        {
            var owner = _registry.GetOwnerWithError();
            var handController = owner == null 
                ? EmptyGamePlayerHandController.Instance 
                : owner.HandController;

            return new GamePlayerHandViewModel(handController);
        }
    }
}