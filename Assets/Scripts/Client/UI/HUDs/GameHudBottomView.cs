using Client.UI.Dialogs.Game.Hand;
using Client.UI.HUDs.ViewModels;
using Reactivity;
using UnityEngine;
using VContainer;

namespace Client.UI.HUDs
{
    public class GameHudBottomView : MonoBehaviour
    {
        [SerializeField]
        private GameHudBottomSwitchesView _switchesView = null!;

        [SerializeField]
        private GamePlayerHandView _playerHandView = null!;
        
        private IGameHudBottomViewModel _viewModel = null!;
        
        public void Init(IObjectResolver resolver, IGameHudBottomViewModel viewModel)
        {
            gameObject.UpdateChildViewModel(ref _viewModel, viewModel);
            resolver.Inject(_playerHandView);
            _playerHandView.Init(_viewModel.PlayerHandViewModel);
            _switchesView.Init(_viewModel.SwitchesViewModel);
        }
    }
}