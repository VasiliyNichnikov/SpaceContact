using System;
using System.Collections.Generic;
using Client.UI;
using Core.Game;
using Core.Game.Phases;
using Network.Game;
using ServiceLayer;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;

namespace App.Game.Services
{
    public class GameStartupService : IStartable, ITickable, IDisposable
    {
        private readonly NetworkManager _networkManager;
        private readonly NetworkGameController _networkGameController;
        private readonly PhaseRegistrationService _phaseRegistrationService;
        private readonly IDialogsManager _dialogsManager;
        private readonly GameNetLoader _gameNetLoader;
        private readonly ContainerRegistrationService _containerRegistrationService;
        private readonly GameStateMachine _gameStateMachine;

        public GameStartupService(
            NetworkManager networkManager,
            PhaseRegistrationService phaseRegistrationService, 
            IDialogsManager dialogsManager,
            NetworkGameController networkGameController,
            GameNetLoader gameNetLoader,
            ContainerRegistrationService containerRegistrationService,
            IObjectResolver gameResolver,
            GameStateMachine gameStateMachine)
        {
            _phaseRegistrationService = phaseRegistrationService;
            _dialogsManager = dialogsManager;
            _networkManager = networkManager;
            _networkGameController = networkGameController;
            _gameNetLoader = gameNetLoader;
            _containerRegistrationService = containerRegistrationService;
            _gameStateMachine = gameStateMachine;
            
            if (_networkManager.SceneManager != null)
            {
                _networkManager.SceneManager.OnLoadEventCompleted += OnSceneLoadCompleted;
            }
            
            _containerRegistrationService.Register(ContainerType.Game, gameResolver);
        }
        
        public void Tick() => 
            _gameStateMachine.Update();
        
        public void Dispose()
        {
            if (_networkManager != null && _networkManager.SceneManager != null)
            {
                _networkManager.SceneManager.OnLoadEventCompleted -= OnSceneLoadCompleted;
            }
            
            _containerRegistrationService.Unregister(ContainerType.Game);
        }
        
        public void Start()
        {
            _dialogsManager.CloseOpenedDialogs();
        }
        
        private void OnSceneLoadCompleted(
            string sceneName, 
            LoadSceneMode loadMode,
            List<ulong> clientsCompleted,
            List<ulong> clientsTimedOut)
        {
            // Регистрация фаз должна быть и на сервере и на клиенте
            _phaseRegistrationService.ConfigureRegistry();
            
            if (_networkManager.IsServer)
            {
                _gameNetLoader.LoadGalaxyNetwork();
                _networkGameController.ServerTransitionTo<GameInitializationPhase>();
            }
        }
    }
}