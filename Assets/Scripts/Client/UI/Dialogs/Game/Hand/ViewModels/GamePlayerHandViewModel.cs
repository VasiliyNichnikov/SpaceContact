using System;
using System.Linq;
using Client.UI.HUDs;
using Client.UI.HUDs.ViewModels;
using Core.Game;
using Reactivity;

namespace Client.UI.Dialogs.Game.Hand.ViewModels
{
    public sealed class GamePlayerHandViewModel : IGamePlayerHandViewModel, IDisposable
    {
        private readonly GameContext _context;
        private readonly ReactivityProperty<bool> _isVisible = new();
        private readonly ReactivityListProperty<IGamePlayerSpaceCardViewModel> _cardsViewModels = new();
        private readonly IGameCurrentPlayerInfoTabController _infoTabController;

        public GamePlayerHandViewModel(GameContext context, IGameCurrentPlayerInfoTabController infoTabController)
        {
            _context = context;
            _infoTabController = infoTabController;
        }

        public IReactivityProperty<bool> IsVisible => _isVisible;

        public IReactivityReadOnlyCollectionProperty<IGamePlayerSpaceCardViewModel> CardsViewModels => _cardsViewModels;

        public void Init()
        {
            _context.StateChanged += RefreshCards;
            _infoTabController.Changed += RefreshCurrentPlayerInfoTab;
            RefreshCards();
            RefreshCurrentPlayerInfoTab();
        }

        public void Dispose()
        {
            _context.StateChanged -= RefreshCards;
            _infoTabController.Changed -= RefreshCurrentPlayerInfoTab;
        }

        private void RefreshCards()
        {
            var localPlayer = _context.LocalPlayerState;
            
            if (localPlayer == null)
            {
                _cardsViewModels.Value = Array.Empty<IGamePlayerSpaceCardViewModel>();
                return;
            }

            var cards = _context.Cards.GetHand(localPlayer.Id).Cards;

            if (cards != null)
            {
                _cardsViewModels.Value = cards.Select(card => new GamePlayerSpaceCardViewModel(card)).ToArray();
            }
            else
            {
                _cardsViewModels.Value = Array.Empty<IGamePlayerSpaceCardViewModel>();
            }
        }

        private void RefreshCurrentPlayerInfoTab()
        {
            _isVisible.Value = _infoTabController.ActiveTab == GamePlayerInfoTabType.CardsDisplay;
        }
    }
}
