using System;
using System.Linq;
using Client.UI.HUDs;
using Client.UI.HUDs.ViewModels;
using Core.Game.Hands;
using Reactivity;

namespace Client.UI.Dialogs.Game.Hand.ViewModels
{
    public class GamePlayerHandViewModel : IGamePlayerHandViewModel, IDisposable
    {
        private readonly ReactivityProperty<bool> _isVisible = new();
        private readonly ReactivityListProperty<IGamePlayerSpaceCardViewModel> _cardsViewModels = new();

        private readonly IGamePlayerHandController _handController;
        private readonly IGameCurrentPlayerInfoTabController _infoTabController;

        public GamePlayerHandViewModel(
            IGamePlayerHandController handController,
            IGameCurrentPlayerInfoTabController infoTabController)
        {
            _handController = handController;
            _infoTabController = infoTabController;
            _handController.OnRefreshed += RefreshCards;
            _infoTabController.Changed += RefreshCurrentPlayerInfoTab;
            
            RefreshCards();
            RefreshCurrentPlayerInfoTab();
        }

        public IReactivityProperty<bool> IsVisible => 
            _isVisible;

        public IReactivityReadOnlyCollectionProperty<IGamePlayerSpaceCardViewModel> CardsViewModels => 
            _cardsViewModels;

        public void Dispose()
        {
            _handController.OnRefreshed -= RefreshCards;
            _infoTabController.Changed -= RefreshCurrentPlayerInfoTab;
        }

        private void RefreshCards()
        {
            var viewModels = _handController
                .SpaceCards
                .Select(card => new GamePlayerSpaceCardViewModel(card))
                .ToList();

            _cardsViewModels.Value = viewModels;
        }

        private void RefreshCurrentPlayerInfoTab()
        {
            _isVisible.Value = _infoTabController.ActiveTab == GamePlayerInfoTabType.CardsDisplay;
        }
    }
}