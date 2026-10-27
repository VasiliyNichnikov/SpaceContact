using System;
using System.Collections.Generic;
using Client.UI;
using Network.Game;
using ServiceLayer;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;

namespace App.Game.Services
{
    public sealed class GameStartupService : IInitializable, IStartable, IDisposable
    {
        private readonly NetworkManager _networkManager;
        private readonly IDialogsManager _dialogsManager;
        private readonly GameNetLoader _gameNetLoader;
        private readonly ContainerRegistrationService _containerRegistrationService;
        private readonly IObjectResolver _resolver;
        private readonly GameStartupNetwork _gameStartupNetwork;

        private NetworkSceneManager? _sceneManager;
        private bool _bootstrapStarted;
        private bool _synchronizationAllowed;
        private bool _disposed;

        public GameStartupService(
            NetworkManager networkManager,
            IDialogsManager dialogsManager,
            GameNetLoader gameNetLoader,
            ContainerRegistrationService containerRegistrationService,
            IObjectResolver resolver,
            GameStartupNetwork gameStartupNetwork)
        {
            _dialogsManager = dialogsManager;
            _networkManager = networkManager;
            _gameNetLoader = gameNetLoader;
            _containerRegistrationService = containerRegistrationService;
            _resolver = resolver;
            _gameStartupNetwork = gameStartupNetwork;
        }

        void IInitializable.Initialize()
        {
            if (_disposed)
            {
                return;
            }

            _sceneManager = _networkManager.SceneManager;
            if (_sceneManager != null)
            {
                _sceneManager.OnLoadEventCompleted += OnSceneLoadCompleted;
            }

            _gameNetLoader.NetworkObjectsLoadedOnAllClients += OnNetworkObjectsLoadedOnAllClients;
            _containerRegistrationService.Register(ContainerType.Game, _resolver);
        }

        void IStartable.Start()
        {
            if (_disposed)
            {
                return;
            }

            _resolver.Resolve<GamePlanetInfoPresenter>();
            _resolver.Resolve<GameClientSynchronizationService>().Start();
            _dialogsManager.CloseOpenedDialogs();
        }

        void IDisposable.Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            UnsubscribeSceneLoad();
            _gameNetLoader.NetworkObjectsLoadedOnAllClients -= OnNetworkObjectsLoadedOnAllClients;
            _containerRegistrationService.Unregister(ContainerType.Game);
        }

        private void OnSceneLoadCompleted(string sceneName, LoadSceneMode loadMode,
            List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
        {
            if (_disposed || _bootstrapStarted || !_networkManager.IsServer)
            {
                return;
            }

            // Claim startup before invoking code that can synchronously call back into this service.
            _bootstrapStarted = true;
            UnsubscribeSceneLoad();
            _resolver.Resolve<GameServerRequestsRegisterService>().ConfigureRegistry();
            var bootstrap = _resolver.Resolve<GameServerBootstrap>();
            bootstrap.Init();
            if (_disposed)
            {
                return;
            }

            _gameNetLoader.LoadNetGame();
            // The loader may have completed before this service subscribed.
            if (_gameNetLoader.IsLoaded)
            {
                OnNetworkObjectsLoadedOnAllClients();
            }
        }

        private void OnNetworkObjectsLoadedOnAllClients()
        {
            if (_disposed || _synchronizationAllowed)
            {
                return;
            }

            _synchronizationAllowed = true;
            _gameNetLoader.NetworkObjectsLoadedOnAllClients -= OnNetworkObjectsLoadedOnAllClients;
            _gameStartupNetwork.AllowSnapshotSynchronization();
        }

        private void UnsubscribeSceneLoad()
        {
            if (_sceneManager == null)
            {
                return;
            }

            _sceneManager.OnLoadEventCompleted -= OnSceneLoadCompleted;
            _sceneManager = null;
        }
    }
}
