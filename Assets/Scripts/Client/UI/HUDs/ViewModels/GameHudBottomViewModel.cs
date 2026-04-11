using Client.Game.Field;
using Client.UI.Dialogs.Game.Hand.ViewModels;
using Core.Game.Hands;
using Core.Game.Players;
using Reactivity;

namespace Client.UI.HUDs.ViewModels
{
    public sealed class GameHudBottomViewModel : IGameHudBottomViewModel
    {
        private readonly ReactivityProperty<bool> _isHandVisible = new();
        
        private readonly GamePlayersRegistry _registry;

        public GameHudBottomViewModel(
            GamePlayersRegistry registry,
            GameFieldPlanetsViewProvider planetsViewProvider)
        {
            _registry = registry;
            PlayerHandViewModel = CreatePlayerHandViewModel();
            SwitchesViewModel = new GameHudBottomSwitchesViewModel(
                planetsViewProvider, 
                value => _isHandVisible.Value = value);
        }

        public IReactivityProperty<bool> IsHandVisible => 
            _isHandVisible;

        public GameHudBottomSwitchesViewModel SwitchesViewModel { get; }

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