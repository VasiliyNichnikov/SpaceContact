using System;
using Client.UI;
using Client.UI.Dialogs.Lobby;
using Unity.Netcode;

namespace App.Services
{
    public class LobbyService : ILobbyController, IDisposable
    {
        private readonly NetworkManager _networkManager;
        private readonly IDialogsManager _dialogsManager;

        public LobbyService(NetworkManager networkManager, IDialogsManager dialogsManager)
        {
            _networkManager = networkManager;
            _dialogsManager = dialogsManager;

            _networkManager.OnServerStarted += OnHostStarted;
            _networkManager.OnClientConnectedCallback += OnLocalClientConnected;
        }
        
        bool ILobbyController.IsOwnerLobby => 
            _networkManager.LocalClientId == NetworkManager.ServerClientId;
        
        void ILobbyController.StartHost()
        {
            if (!_networkManager.IsListening)
            {
                _networkManager.StartHost();
            }
        }

        void ILobbyController.StartClient()
        {
            if (!_networkManager.IsListening)
            {
                _networkManager.StartClient();
            }
        }
        
        public void Dispose()
        {
            if (_networkManager == null)
            {
                return;
            }
            
            _networkManager.OnServerStarted -= OnHostStarted;
            _networkManager.OnClientConnectedCallback -= OnLocalClientConnected;
        }

        private void OnHostStarted()
        {
            _dialogsManager.ShowDialog<LobbyDialog>();
        }

        private void OnLocalClientConnected(ulong clientId)
        {
            if (clientId == _networkManager.LocalClientId)
            {
                _dialogsManager.ShowDialog<LobbyDialog>();
            }
        }
    }
}