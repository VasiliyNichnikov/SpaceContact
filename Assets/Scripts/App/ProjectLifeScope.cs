using App.Configs;
using App.Services;
using Client;
using Client.Configs;
using Client.Configs.Menu;
using Client.Factory;
using Client.UI;
using Client.UI.Dialogs;
using Client.UI.Dialogs.Lobby;
using Core;
using Core.Lobby;
using Core.Player;
using Network;
using Network.Configs;
using Network.Infrastructure;
using Network.Requests;
using ServiceLayer;
using Unity.Netcode;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace App
{
    public class ProjectLifeScope : LifetimeScope
    {
        [SerializeField]
        private NetworkManager _networkManager = null!;
        
        [SerializeField]
        private DialogsRegistrySO _dialogsRegistrySO = null!;
        
        [SerializeField]
        private SceneStorage _sceneStorage = null!;

        [SerializeField] 
        private AppConfigs _appConfigs = null!;
        
        [SerializeField]
        private MenuConfigs _menuConfigs = null!;
        
        [SerializeField]
        private SimpleConnectionDialog _simpleConnectionDialog = null!;
        
        [SerializeField]
        private ProjectNetworkRegistrySO _projectNetworkRegistrySo = null!;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(_networkManager);
            builder.RegisterComponent(_simpleConnectionDialog);
            builder.RegisterInstance(_projectNetworkRegistrySo);
            
            // instance - не будет искать в объект inject
            builder.RegisterInstance(_dialogsRegistrySO);
            builder.RegisterInstance(_sceneStorage);
            
            // Configs
            _appConfigs.Build(builder);
            _menuConfigs.Build(builder);

            // Singletons
            builder.Register<JsonNetworkSerializer>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<NetworkAutoInjector>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<PlayersRegistry>(Lifetime.Singleton).AsSelf();
            builder.Register<GameLevelService>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<DialogsManager>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<ContainerRegistrationService>(Lifetime.Singleton).AsSelf();
            builder.Register<LobbyColorProvider>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<ProjectNetLoader>(Lifetime.Singleton).AsSelf();
            builder.Register<CoreNetworkContext>(Lifetime.Singleton).AsSelf();
            
            // Factories
            builder.Register<DialogsFactory>(Lifetime.Singleton);
            
            // Services
            builder.Register<LobbyService>(Lifetime.Singleton).AsImplementedInterfaces();
            
            // ViewModels
            RegisterViewModels(builder);
            
            // Requests
            builder.Register<NetworkRequestRouter>(Lifetime.Singleton).AsSelf();
            builder.Register<NetworkService>(Lifetime.Singleton).AsImplementedInterfaces();
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