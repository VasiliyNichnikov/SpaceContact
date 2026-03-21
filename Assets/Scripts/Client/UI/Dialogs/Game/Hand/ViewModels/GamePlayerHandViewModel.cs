using System;
using System.Linq;
using Core.Game.Hands;
using Reactivity;

namespace Client.UI.Dialogs.Game.Hand.ViewModels
{
    public class GamePlayerHandViewModel : IGamePlayerHandViewModel, IDisposable
    {
        private readonly ReactivityListProperty<IGamePlayerSpaceCardViewModel> _cardsViewModels = new();

        private readonly IGamePlayerHandController _handController;

        public GamePlayerHandViewModel(IGamePlayerHandController handController)
        {
            _handController = handController;
            _handController.OnRefreshed += RefreshCards;
            RefreshCards();
        }
        
        public IReactivityReadOnlyCollectionProperty<IGamePlayerSpaceCardViewModel> CardsViewModels => 
            _cardsViewModels;

        public void Dispose()
        {
            _handController.OnRefreshed -= RefreshCards;
        }

        private void RefreshCards()
        {
            var viewModels = _handController
                .SpaceCards
                .Select(card => new GamePlayerSpaceCardViewModel(card))
                .ToList();

            _cardsViewModels.Value = viewModels;
        }
    }
}