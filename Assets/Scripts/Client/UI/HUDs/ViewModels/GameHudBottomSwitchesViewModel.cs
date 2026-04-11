using System.Collections.Generic;

namespace Client.UI.HUDs.ViewModels
{
    public sealed class GameHudBottomSwitchesViewModel
    {
        private readonly GameCurrentPlayerInfoTabController _infoTabController;
        
        public GameHudBottomSwitchesViewModel(GameCurrentPlayerInfoTabController infoTabController)
        {
            _infoTabController = infoTabController;
            Buttons = CreateButtons();
            ShowCards();
        }
        
        public IReadOnlyCollection<GameHudBottomSwitchButtonViewModel> Buttons { get; }

        private IReadOnlyCollection<GameHudBottomSwitchButtonViewModel> CreateButtons()
        {
            var showCardsButton = new GameHudBottomSwitchButtonViewModel(GamePlayerInfoTabType.CardsDisplay, ShowCards);
            var showPlanetsButton = new GameHudBottomSwitchButtonViewModel(GamePlayerInfoTabType.PlanetsDisplay, ShowPlanets);

            return new [] { showCardsButton, showPlanetsButton };
        }

        private void ShowCards()
        {
            _infoTabController.ShowCards();
            ShowSelectedButton();
        }

        private void ShowPlanets()
        {
            _infoTabController.ShowPlanets();
            ShowSelectedButton();
        }

        private void ShowSelectedButton()
        {
            foreach (var button in Buttons)
            {
                if (button.Type == _infoTabController.ActiveTab)
                {
                    button.Select();
                }
                else
                {
                    button.Deselect();
                }
            }
        }
    }
}