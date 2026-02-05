using App.Factory;
using App.Services;
using Client;
using Client.Factory;
using Client.Phases;
using Client.UI.Dialogs.Lobby;
using Configs.UI;
using Core;
using Core.Factory;
using Core.Phases;
using Network;
using Network.Infrastructure;
using Network.Player;
using Unity.Netcode;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace App
{
    public class GameLifeScope : LifetimeScope
    {
        [SerializeField]
        private NetworkManager _networkManager = null!;
        
        [SerializeField]
        private NetworkGameController _networkGameController = null!;
        
        [SerializeField]
        private DialogsRegistrySO _dialogsRegistrySO = null!;
        
        [SerializeField]
        private SceneStorage _sceneStorage = null!;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(_networkManager);
            builder.RegisterComponent(_networkGameController);
            
            // instance - не будет искать в объект inject
            builder.RegisterInstance(_dialogsRegistrySO);
            builder.RegisterInstance(_sceneStorage);

            // Singletons
            builder.Register<PhaseRegistry>(Lifetime.Singleton).AsSelf();
            builder.Register<JsonNetworkSerializer>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<NetworkAutoInjector>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<PhaseRegistrationService>(Lifetime.Singleton).AsSelf();
            builder.Register<PlayersRegistry>(Lifetime.Singleton).AsSelf();
            
            // Binders
            builder.Register<UIPhasesBinder>(Lifetime.Singleton).AsImplementedInterfaces();
            
            // Factories
            builder.Register<VContainerPhasesFactory>(Lifetime.Singleton).As<IPhaseFactory>();
            builder.Register<DialogsFactory>(Lifetime.Singleton);
            
            // Game State Machine
            builder.Register<GameStateMachine>(Lifetime.Singleton).AsSelf();
            
            // Phases
            RegisterPhases(builder);
            
            // ViewModels
            RegisterViewModels(builder);
            
            // Entry Point
            builder.RegisterEntryPoint<GameStartupService>();
        }

        private static void RegisterPhases(IContainerBuilder builder)
        {
            builder.Register<LobbyPhase>(Lifetime.Transient);
            builder.Register<RegroupPhase>(Lifetime.Transient);
        }

        private static void RegisterViewModels(IContainerBuilder builder)
        {
            RegisterViewModel<LobbyViewModel>(builder);
        }

        private static void RegisterViewModel<T>(IContainerBuilder builder)
        {
            builder.Register<T>(Lifetime.Transient);
        }
    }
}