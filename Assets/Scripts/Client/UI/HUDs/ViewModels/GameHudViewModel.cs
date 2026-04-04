using Client.Game.Field;
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
        private readonly ReactivityProperty<GamePlayerBlockViewModel> _playerBlockViewModel = new();
        
        private readonly GamePlayersRegistry _registry;
        private readonly IGameClientDestinyCardController _destinyCardController;
        private readonly IGameFieldViewManager _fieldViewManager;
        
        public GameHudViewModel(
            GamePlayersRegistry registry, 
            IGameHudTopViewModel topViewModel,
            IGameClientDestinyCardController destinyCardController,
            IGameFieldViewManager fieldViewManager)
        {
            _registry = registry;
            TopViewModel = topViewModel;
            _destinyCardController = destinyCardController;
            _fieldViewManager = fieldViewManager;
            PlayerHandViewModel = CreatePlayerHandViewModel();
            _fieldViewManager.OnViewedOpponentChanged += OpponentChanged;
            _destinyCardController.Changed += OnDestinyCardChanged;
            OpponentChanged();
        }

        public IGameHudTopViewModel TopViewModel { get; }
        
        public IGamePlayerHandViewModel PlayerHandViewModel { get; }

        public IReactivityProperty<IGameDestinyCardViewModel> DestinyCardViewModel => 
            _destinyCardViewModel;

        public IReactivityProperty<GamePlayerBlockViewModel> OpponentPlayerViewModel => 
            _playerBlockViewModel;

        public void Dispose()
        {
            TopViewModel.Dispose();
            _destinyCardController.Changed -= OnDestinyCardChanged;
            _fieldViewManager.OnViewedOpponentChanged -= OpponentChanged;
        }

        private IGamePlayerHandViewModel CreatePlayerHandViewModel()
        {
            var owner = _registry.GetOwnerWithError();
            var handController = owner == null 
                ? EmptyGamePlayerHandController.Instance 
                : owner.HandController;

            return new GamePlayerHandViewModel(handController);
        }

        private void OpponentChanged()
        {
            var viewedOpponentPlayer = _fieldViewManager.ViewedOpponentPlayer;

            if (viewedOpponentPlayer == null)
            {
                Logger.Error($"{nameof(GameHudViewModel)}.{nameof(OpponentChanged)}: viewedOpponentPlayer is null.");
                return;
            }
            
            _playerBlockViewModel.Value = new GamePlayerBlockViewModel(viewedOpponentPlayer);
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