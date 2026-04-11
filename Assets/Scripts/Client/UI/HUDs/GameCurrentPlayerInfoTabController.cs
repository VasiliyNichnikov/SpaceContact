using System;
using Client.UI.HUDs.ViewModels;

namespace Client.UI.HUDs
{
    public sealed class GameCurrentPlayerInfoTabController : IGameCurrentPlayerInfoTabController
    {
        private GamePlayerInfoTabType _activeTab;
        
        public event Action? Changed;

        public GamePlayerInfoTabType ActiveTab
        {
            get => _activeTab;
            private set
            {
                if (_activeTab == value)
                {
                    return;
                }
                
                _activeTab = value;
                Changed?.Invoke();
            }
        }

        public void ShowCards() => 
            ActiveTab = GamePlayerInfoTabType.CardsDisplay;

        public void ShowPlanets() => 
            ActiveTab = GamePlayerInfoTabType.PlanetsDisplay;
    }
}