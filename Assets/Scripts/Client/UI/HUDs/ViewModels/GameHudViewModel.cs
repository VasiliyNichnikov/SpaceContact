using System.Collections.Generic;
using System.Linq;
using Client.Game.Field;
using Client.UI.Dialogs.Game.Hand.ViewModels;
using Core.Game.Hands;
using Core.Game.Phases.Client;
using Core.Game.Players;
using Core.Game.Rules;
using GeneralUtils;
using Logs;
using Reactivity;

namespace Client.UI.HUDs.ViewModels
{
    public sealed class GameHudViewModel : IGameHudViewModel
    {
        private readonly ReactivityProperty<IGameDestinyCardViewModel> _destinyCardViewModel = new();
        private readonly ReactivityProperty<GamePlayerBlockViewModel> _playerBlockViewModel = new();
        
        private readonly GamePlayersRegistry _registry;
        private readonly IGameClientDestinyPhaseResolver _destinyPhaseResolver;
        private readonly IGameFieldViewManager _fieldViewManager;
        private readonly List<GamePlayerProfileViewModel> _playerProfilesViewModels;
        private readonly GameRulesChecker _rulesChecker;
        
        public GameHudViewModel(
            GamePlayersRegistry registry, 
            IGameHudTopViewModel topViewModel,
            IGameClientDestinyPhaseResolver destinyPhaseResolver,
            IGameFieldViewManager fieldViewManager,
            GameRulesChecker rulesChecker)
        {
            _registry = registry;
            TopViewModel = topViewModel;
            _destinyPhaseResolver = destinyPhaseResolver;
            _fieldViewManager = fieldViewManager;
            _rulesChecker = rulesChecker;
            PlayerHandViewModel = CreatePlayerHandViewModel();
            _fieldViewManager.OnViewedOpponentChanged += OpponentChanged;
            _fieldViewManager.OnInitialized += OpponentChanged;
            _destinyPhaseResolver.Changed += OnDestinyCardChanged;
            _playerProfilesViewModels = CreatePlayerProfilesViewModels();
            OpponentChanged();
        }

        public IGameHudTopViewModel TopViewModel { get; }
        
        public IGamePlayerHandViewModel PlayerHandViewModel { get; }

        public IReactivityProperty<IGameDestinyCardViewModel> DestinyCardViewModel => 
            _destinyCardViewModel;

        public IReactivityProperty<GamePlayerBlockViewModel> OpponentPlayerViewModel => 
            _playerBlockViewModel;

        public IReadOnlyCollection<GamePlayerProfileViewModel> PlayerProfilesViewModels => 
            _playerProfilesViewModels;

        public void Dispose()
        {
            TopViewModel.Dispose();
            _destinyPhaseResolver.Changed -= OnDestinyCardChanged;
            _fieldViewManager.OnInitialized -= OpponentChanged;
            _fieldViewManager.OnViewedOpponentChanged -= OpponentChanged;

            foreach (var playerProfileViewModel in _playerProfilesViewModels)
            {
                playerProfileViewModel.Dispose();
            }
            
            _playerProfilesViewModels.Clear();
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
            var activeDestinyCard = _destinyPhaseResolver.Card;

            if (activeDestinyCard == null)
            {
                Logger.Error($"{nameof(GameHudViewModel)}.{nameof(OnDestinyCardChanged)}: activeDestinyCard is null.");
                
                return;
            }
            
            var owner = _registry.GetOwnerWithError();

            if (owner == null)
            {
                return;
            }
            
            _destinyCardViewModel.Value = new GameDestinyCardViewModel(
                owner,
                activeDestinyCard,
                _rulesChecker,
                SkipDestinyCard);
        }

        private List<GamePlayerProfileViewModel> CreatePlayerProfilesViewModels()
        {
            return _registry.Players.Select(player => new GamePlayerProfileViewModel(player)).ToList();
        }

        private void SkipDestinyCard()
        {
            if (_destinyPhaseResolver.IsWaitingServer)
            {
                return;
            }
            
            _destinyPhaseResolver.SkipDestinyAsync().FireAndForget();
        }
    }
}