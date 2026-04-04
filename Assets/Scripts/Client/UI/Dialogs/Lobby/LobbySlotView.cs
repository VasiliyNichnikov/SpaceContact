using Client.UI.Dialogs.Lobby.ViewModels;
using UnityEngine;
using Logger = Logs.Logger;

namespace Client.UI.Dialogs.Lobby
{
    public class LobbySlotView : MonoBehaviour
    {
        [SerializeField]
        private LobbyPlayerSlotView _lobbyPlayerSlotViewPrefab = null!;
        
        [SerializeField]
        private LobbyEmptySlotView _lobbyEmptySlotViewPrefab = null!;
        
        private LobbyPlayerSlotView? _createdLobbySlotPlayerView;
        private LobbyEmptySlotView? _createdLobbyEmptySlotView;
        
        public void Init(ILobbySlotViewModel viewModel)
        {
            switch (viewModel)
            {
                case LobbyEmptySlotViewModel emptySlot:
                    InitEmptySlotView(emptySlot);
                    break;
                
                case LobbyPlayerViewModel player:
                    InitPlayerSlotView(player);
                    break;
                
                default:
                    Logger.Error($"Unexpected view model type {viewModel.GetType()}");
                    break;
            }
        }
        
        private void InitEmptySlotView(LobbyEmptySlotViewModel viewModel)
        {
            if (_createdLobbySlotPlayerView != null)
            {
                _createdLobbySlotPlayerView.Hide();
            }
            
            if (_createdLobbyEmptySlotView == null)
            {
                _createdLobbyEmptySlotView = Instantiate(_lobbyEmptySlotViewPrefab, transform, false);
            }
            
            _createdLobbyEmptySlotView.Show();
            _createdLobbyEmptySlotView.Init(viewModel);
        }

        private void InitPlayerSlotView(LobbyPlayerViewModel viewModel)
        {
            if (_createdLobbyEmptySlotView != null)
            {
                _createdLobbyEmptySlotView.Hide();
            }

            if (_createdLobbySlotPlayerView == null)
            {
                _createdLobbySlotPlayerView = Instantiate(_lobbyPlayerSlotViewPrefab, transform, false);
            }
            
            _createdLobbySlotPlayerView.Show();
            _createdLobbySlotPlayerView.Init(viewModel);
        }
    }
}