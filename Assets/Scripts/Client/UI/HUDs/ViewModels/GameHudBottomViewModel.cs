using Client.UI.Dialogs.Game.Hand.ViewModels;
using Core.Game.Hands;
using Core.Game.Players;

namespace Client.UI.HUDs.ViewModels
{
    public sealed class GameHudBottomViewModel : IGameHudBottomViewModel
    {
        private readonly GamePlayersRegistry _registry;

        public GameHudBottomViewModel(
            GamePlayersRegistry registry,
            GameCurrentPlayerInfoTabController infoTabController)
        {
            _registry = registry;
            PlayerHandViewModel = CreatePlayerHandViewModel(infoTabController);
            SwitchesViewModel = new GameHudBottomSwitchesViewModel(infoTabController);
        }

        public GameHudBottomSwitchesViewModel SwitchesViewModel { get; }

        public IGamePlayerHandViewModel PlayerHandViewModel { get; }
        
        private IGamePlayerHandViewModel CreatePlayerHandViewModel(GameCurrentPlayerInfoTabController infoTabController)
        {
            var owner = _registry.GetOwnerWithError();
            var handController = owner == null 
                ? EmptyGamePlayerHandController.Instance 
                : owner.HandController;

            return new GamePlayerHandViewModel(handController, infoTabController);
        }
    }
}