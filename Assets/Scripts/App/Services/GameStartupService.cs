using System;
using Core.Phases;
using Logs;
using Network;
using Unity.Netcode;
using VContainer.Unity;

namespace App.Services
{
    public class GameStartupService : IStartable, IDisposable
    {
        private readonly PhaseRegistrationService _phaseRegistrationService;
        private readonly NetworkGameController _networkGameController;
        private readonly NetworkManager _networkManager;

        public GameStartupService(
            PhaseRegistrationService phaseRegistrationService,
            NetworkGameController networkGameController,
            NetworkManager networkManager)
        {
            _phaseRegistrationService = phaseRegistrationService;
            _networkGameController = networkGameController;
            _networkManager = networkManager;
        }
        
        public void Start()
        {
            _phaseRegistrationService.ConfigureRegistry();
            _networkManager.OnServerStarted += OnServerStarted;
        }

        public void Dispose()
        {
            _networkManager.OnServerStarted -= OnServerStarted;
        }

        private void OnServerStarted()
        {
            if (!_networkManager.IsServer)
            {
                return;
            }
            
            Logger.Log("GameStartupService.OnServerStarted: server have started.");
            _networkGameController.ServerTransitionTo<RegroupPhase>();
        }
    }
}