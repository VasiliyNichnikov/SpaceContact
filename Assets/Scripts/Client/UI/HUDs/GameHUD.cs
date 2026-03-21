using Client.UI.Dialogs.Game.Hand;
using Client.UI.HUDs.ViewModels;
using Reactivity;
using UnityEngine;
using VContainer;

namespace Client.UI.HUDs
{
    public class GameHUD : MonoBehaviour
    {
        [SerializeField]
        private GamePlayerHandView _playerHandView = null!;

        private IObjectResolver _resolver = null!;
        private IGameHudViewModel _viewModel = null!;
        
        [Inject]
        private void Construct(IObjectResolver resolver, IGameHudViewModel viewModel)
        {
            gameObject.UpdateViewModelSimple(ref _viewModel, viewModel);
            _resolver = resolver;
            _resolver.Inject(_playerHandView);
        }

        public void Init()
        {
            _playerHandView.Init(_viewModel.PlayerHandViewModel);
        }
    }
}