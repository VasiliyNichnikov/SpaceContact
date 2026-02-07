using System.Linq;
using Client.UI.Utils;
using Reactivity;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Client.UI.Dialogs.Lobby
{
    public class LobbyDialog : BaseDialog, IStartable
    {
        [SerializeField]
        private RectTransform _playersContainer = null!;
        
        [SerializeField]
        private LobbyPlayerView _lobbyPlayerView = null!;

        private LobbyViewModel _viewModel = null!;
        
        [Inject]
        private void Constructor(LobbyViewModel viewModel)
        {
            gameObject.UpdateViewModelDisposable(ref _viewModel, viewModel);
        }
        
        public void Start()
        {
            gameObject.Subscribe(_viewModel.RefreshPlayersEvent, RefreshPlayersBlocks);
        }

        private void RefreshPlayersBlocks()
        {
            var viewModels = _viewModel.Players.ToList();
            UIUtils.CreateRequiredNumberOfItems(
                _playersContainer,
                _lobbyPlayerView,
                viewModels,
                (view, viewModel) => { view.Init(viewModel); });
        }
    }
}