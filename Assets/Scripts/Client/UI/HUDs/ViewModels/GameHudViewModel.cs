using Client.UI.Dialogs.Game.Hand.ViewModels;
using Core.Game.Cards;
using Core.Game.Hands;
using Core.Game.Players;
using Logs;
using Reactivity;

namespace Client.UI.HUDs.ViewModels
{
    public class GameHudViewModel : IGameHudViewModel
    {
        private readonly ReactivityProperty<IGameDestinyCardViewModel> _destinyCardViewModel = new();
        
        private readonly GamePlayersRegistry _registry;
        private readonly IGameClientDestinyCardController _destinyCardController;
        
        public GameHudViewModel(
            GamePlayersRegistry registry, 
            IGameHudTopViewModel topViewModel,
            IGameClientDestinyCardController destinyCardController)
        {
            _registry = registry;
            TopViewModel = topViewModel;
            _destinyCardController = destinyCardController;
            PlayerHandViewModel = CreatePlayerHandViewModel();
            _destinyCardController.Changed += OnDestinyCardChanged;
        }

        public IGameHudTopViewModel TopViewModel { get; }
        
        public IGamePlayerHandViewModel PlayerHandViewModel { get; }

        public IReactivityProperty<IGameDestinyCardViewModel> DestinyCardViewModel => 
            _destinyCardViewModel;
        
        public void Dispose()
        {
            TopViewModel.Dispose();
            _destinyCardController.Changed -= OnDestinyCardChanged;
        }

        private IGamePlayerHandViewModel CreatePlayerHandViewModel()
        {
            var owner = _registry.GetOwnerWithError();
            var handController = owner == null 
                ? EmptyGamePlayerHandController.Instance 
                : owner.HandController;

            return new GamePlayerHandViewModel(handController);
        }
        
        private void OnDestinyCardChanged()
        {
            var activeDestinyCard = _destinyCardController.Card;

            if (activeDestinyCard == null)
            {
                Logger.Error("GameHudViewModel.OnDestinyCardChanged: activeDestinyCard is null.");
                
                return;
            }
            
            _destinyCardViewModel.Value = new GameDestinyCardViewModel(activeDestinyCard);
        }
    }
}