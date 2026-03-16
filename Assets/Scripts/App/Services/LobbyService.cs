using System;
using Client.UI;
using Client.UI.Dialogs.Lobby;
using Core;
using Network;
using Unity.Netcode;

namespace App.Services
{
    public class LobbyService : ILobbyController, IDisposable
    {
        private readonly NetworkManager _networkManager;
        private readonly IDialogsManager _dialogsManager;
        private readonly ProjectNetLoader _projectNetLoader;
        private readonly CoreNetworkContext _networkContext;

        public LobbyService(
            NetworkManager networkManager, 
            IDialogsManager dialogsManager,
            ProjectNetLoader projectNetLoader,
            CoreNetworkContext networkContext)
        {
            _networkManager = networkManager;
            _dialogsManager = dialogsManager;
            _projectNetLoader = projectNetLoader;
            _networkContext = networkContext;

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
                _projectNetLoader.LoadNetProject();
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
            _networkContext.SetThisServer().SetThisOwner();
            _dialogsManager.ShowDialog<LobbyDialog>();
        }

        private void OnLocalClientConnected(ulong clientId)
        {
            if (clientId == _networkManager.LocalClientId)
            {
                _networkContext.SetThisOwner();
                _dialogsManager.ShowDialog<LobbyDialog>();
            }
        }
    }
}