using System.Collections.Generic;
using System.Linq;
using Client.UI.Dialogs.Game.Hand;
using Client.UI.HUDs.ViewModels;
using Client.UI.Utils;
using Reactivity;
using UnityEngine;
using VContainer;

namespace Client.UI.HUDs
{
    public class GameHUD : MonoBehaviour
    {
        [SerializeField]
        private GameHudTopView _topView = null!;
        
        [SerializeField]
        private GamePlayerHandView _playerHandView = null!;
        
        [SerializeField]
        private GameDestinyCardView _gameDestinyCard = null!;
        
        [SerializeField]
        private GameOpponentPlayerBlockView _gameOpponentPlayerBlockView = null!;

        [SerializeField]
        private RectTransform _playerProfilesContainer = null!;
        
        [SerializeField]
        private GamePlayerProfileView _gamePlayerProfileViewPrefab = null!;
        
        private IObjectResolver _resolver = null!;
        private IGameHudViewModel _viewModel = null!;
        
        [Inject]
        private void Construct(IObjectResolver resolver, IGameHudViewModel viewModel)
        {
            gameObject.UpdateViewModelDisposable(ref _viewModel, viewModel);
            gameObject.SubscribeWithoutCall(_viewModel.DestinyCardViewModel, _gameDestinyCard.Init);
            gameObject.Subscribe(_viewModel.OpponentPlayerViewModel, _gameOpponentPlayerBlockView.Refresh);
            _topView.Init(viewModel.TopViewModel);
            InitGamePlayerProfiles(viewModel.PlayerProfilesViewModels);
            _resolver = resolver;
            _resolver.Inject(_playerHandView);
        }

        public void Init()
        {
            _playerHandView.Init(_viewModel.PlayerHandViewModel);
        }

        private void InitGamePlayerProfiles(IReadOnlyCollection<GamePlayerProfileViewModel> viewModels)
        {
            var viewModelsAsList = viewModels.ToList();
            UIUtils.CreateRequiredNumberOfItems(
                _playerProfilesContainer, 
                _gamePlayerProfileViewPrefab,
                viewModelsAsList,
                (view, viewModel) =>
                {
                    view.Init(viewModel);
                });
        }
    }
}