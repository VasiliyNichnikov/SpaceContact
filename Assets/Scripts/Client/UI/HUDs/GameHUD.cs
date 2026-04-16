using System.Collections.Generic;
using System.Linq;
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
        private GameHudBottomView _bottomView = null!;
        
        [SerializeField]
        private GameDestinyCardView _gameDestinyCard = null!;
        
        [SerializeField]
        private GameOpponentPlayerBlockView _gameOpponentPlayerBlockView = null!;

        [SerializeField]
        private RectTransform _playerProfilesContainer = null!;
        
        [SerializeField]
        private GamePlayerProfileView _gamePlayerProfileViewPrefab = null!;
        
        private IGameHudViewModel _viewModel = null!;
        
        [Inject]
        private void Construct(IObjectResolver resolver, IGameHudViewModel viewModel)
        {
            gameObject.UpdateViewModelDisposable(ref _viewModel, viewModel);
            gameObject.Subscribe(_viewModel.DestinyCardViewModel, InitDestinyCardViewModel);
            gameObject.Subscribe(_viewModel.OpponentPlayerViewModel, _gameOpponentPlayerBlockView.Refresh);
            _topView.Init(viewModel.TopViewModel);
            _bottomView.Init(resolver, viewModel.BottomViewModel);
            InitGamePlayerProfiles(viewModel.PlayerProfilesViewModels);
        }

        private void InitDestinyCardViewModel(IGameDestinyCardViewModel? viewModel)
        {
            if (viewModel == null)
            {
                _gameDestinyCard.Hide();
                
                return;
            }
            
            _gameDestinyCard.Show();
            _gameDestinyCard.Init(viewModel);
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