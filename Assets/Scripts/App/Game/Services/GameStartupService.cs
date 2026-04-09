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
        private readonly IServerStateMachineNetwork _serverStateMachineNetwork;
        private readonly PhaseRegistrationService _phaseRegistrationService;
        private readonly IDialogsManager _dialogsManager;
        private readonly GameNetLoader _gameNetLoader;
        private readonly ContainerRegistrationService _containerRegistrationService;
        private readonly GameStateMachine _gameStateMachine;
        private readonly GamePlayersLoader _gamePlayersLoader;
        private readonly IObjectResolver _resolver;

        public GameStartupService(
            NetworkManager networkManager,
            PhaseRegistrationService phaseRegistrationService,
            IDialogsManager dialogsManager,
            IServerStateMachineNetwork serverStateMachineNetwork,
            GameNetLoader gameNetLoader,
            ContainerRegistrationService containerRegistrationService,
            IObjectResolver resolver,
            GameStateMachine gameStateMachine,
            GamePlayersLoader gamePlayersLoader)
        {
            _phaseRegistrationService = phaseRegistrationService;
            _dialogsManager = dialogsManager;
            _networkManager = networkManager;
            _serverStateMachineNetwork = serverStateMachineNetwork;
            _gameNetLoader = gameNetLoader;
            _containerRegistrationService = containerRegistrationService;
            _gameStateMachine = gameStateMachine;
            _gamePlayersLoader = gamePlayersLoader;
            _resolver = resolver;
            
            if (_networkManager.SceneManager != null)
            {
                _networkManager.SceneManager.OnLoadEventCompleted += OnSceneLoadCompleted;
            }
            
            _gameNetLoader.OnGameIsReady += GameReady;
            _containerRegistrationService.Register(ContainerType.Game, resolver);
        }
        
        void ITickable.Tick() => 
            _gameStateMachine.Update();
        
        void IStartable.Start()
        {
            // Если не сделать - код не придет в исполнение
            _resolver.Resolve<GamePlanetInfoPresenter>();
            _dialogsManager.CloseOpenedDialogs();
        }
        
        void IDisposable.Dispose()
        {
            if (_networkManager != null && _networkManager.SceneManager != null)
            {
                _networkManager.SceneManager.OnLoadEventCompleted -= OnSceneLoadCompleted;
            }
            
            _containerRegistrationService.Unregister(ContainerType.Game);
            _gameNetLoader.OnGameIsReady -= GameReady;
        }
        
        private void OnSceneLoadCompleted(
            string sceneName, 
            LoadSceneMode loadMode,
            List<ulong> clientsCompleted,
            List<ulong> clientsTimedOut)
        {
            // Регистрация фаз должна быть и на сервере и на клиенте
            _phaseRegistrationService.ConfigureRegistry();
            
            // Загружаем игроков на клиенте и сервере
            _gamePlayersLoader.Init();
            
            // Загружаем все игровые объекты
            if (_networkManager.IsServer)
            {
                // Регистрируем игровые запросы
                var requestsRegisterService = _resolver.Resolve<GameServerRequestsRegisterService>();
                requestsRegisterService.ConfigureRegistry();
                // Грузим основной Core
                var serverCoreLoader = _resolver.Resolve<GameServerCoreLoader>();
                serverCoreLoader.Init();
                // Затем на основе Core грузится Net объекты
                _gameNetLoader.LoadNetGame();
            }
        }

        private void GameReady()
        {
            if (!_networkManager.IsServer)
            {
                return;
            }
            
            _serverStateMachineNetwork.ServerTransitionTo<GameInitializationPhase>();
        }
    }
}