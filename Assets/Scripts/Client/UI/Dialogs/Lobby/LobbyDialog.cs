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
        private LobbySlotView _slotView = null!;
        
        [SerializeField]
        private GameObject _startGameButtonGameObject = null!;

        private LobbyViewModel _viewModel = null!;
        
        [Inject]
        private void Constructor(LobbyViewModel viewModel)
        {
            gameObject.UpdateViewModelDisposable(ref _viewModel, viewModel);
        }
        
        public void Start()
        {
            gameObject.Subscribe(_viewModel.RefreshPlayersEvent, RefreshPlayersBlocks);
            _startGameButtonGameObject.SetActive(_viewModel.IsOwnerLobby);
        }

        /// <summary>
        /// Called from Unity
        /// </summary>
        public void OnRunBattleButtonClick() => 
            _viewModel.RunBattleButtonClickHandler();

        private void RefreshPlayersBlocks()
        {
            var viewModels = _viewModel.Slots.ToList();

            if (viewModels.Count == 0)
            {
                return;
            }
            
            UIUtils.CreateRequiredNumberOfItems(
                _playersContainer,
                _slotView,
                viewModels,
                (view, viewModel) => { view.Init(viewModel); });
        }
    }
}