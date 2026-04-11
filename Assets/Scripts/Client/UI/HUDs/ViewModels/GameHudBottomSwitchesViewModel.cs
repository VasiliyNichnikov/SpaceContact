using System;
using System.Collections.Generic;
using Client.Game.Field;

namespace Client.UI.HUDs.ViewModels
{
    public sealed class GameHudBottomSwitchesViewModel
    {
        private readonly GameFieldPlanetsViewProvider _planetsViewProvider;
        private readonly Action<bool> _changeCardsDisplayAction;
        
        public GameHudBottomSwitchesViewModel(
            GameFieldPlanetsViewProvider planetsViewProvider, 
            Action<bool> changeCardsDisplayAction)
        {
            _planetsViewProvider = planetsViewProvider;
            _changeCardsDisplayAction = changeCardsDisplayAction;
            Buttons = CreateButtons();
            ShowCards();
        }
        
        public IReadOnlyCollection<GameHudBottomSwitchButtonViewModel> Buttons { get; }

        private IReadOnlyCollection<GameHudBottomSwitchButtonViewModel> CreateButtons()
        {
            var showCardsButton = new GameHudBottomSwitchButtonViewModel(GameHudBottomSwitchButtonType.CardsDisplay, ShowCards);
            var showPlanetsButton = new GameHudBottomSwitchButtonViewModel(GameHudBottomSwitchButtonType.PlanetsDisplay, ShowPlanets);

            return new [] { showCardsButton, showPlanetsButton };
        }

        private void ShowCards()
        {
            _changeCardsDisplayAction.Invoke(true);
            _planetsViewProvider.HidePlayerPlanets();
            ShowSelectedButton(GameHudBottomSwitchButtonType.CardsDisplay);
        }

        private void ShowPlanets()
        {
            _changeCardsDisplayAction.Invoke(false);
            _planetsViewProvider.ShowPlayerPlanets();
            ShowSelectedButton(GameHudBottomSwitchButtonType.PlanetsDisplay);
        }

        private void ShowSelectedButton(GameHudBottomSwitchButtonType type)
        {
            foreach (var button in Buttons)
            {
                if (button.Type == type)
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