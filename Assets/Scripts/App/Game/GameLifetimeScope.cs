using App.Game.Services;
using Client.Configs.Game;
using Client.Game.Factory;
using Client.Game.Field;
using Client.Game.Planets.ViewModels;
using Client.Helpers;
using Client.UI;
using Client.UI.Dialogs.Game.Hand.ViewModels;
using Client.UI.Dialogs.Game.PlayerChoice.ViewModels;
using Client.UI.HUDs;
using Client.UI.HUDs.ViewModels;
using Client.UI.Loaders;
using Core.Game;
using Core.Game.Actions;
using Core.Game.Cards;
using Core.Game.Events;
using Core.Game.Galaxy;
using Core.Game.InitialSnapshotFactory;
using Core.Game.Players;
using Core.Game.Server;
using Network.Configs;
using Network.Game;
using Unity.Netcode;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using GameFieldManager = Core.Game.GameFieldManager;

namespace App.Game
{
    public class GameLifetimeScope : LifetimeScope
    {
        [SerializeField]
        private GameRevisionAckNetwork _revisionAckNetwork = null!;

        [SerializeField]
        private GameStartupNetwork _startupNetwork = null!;
        
        [SerializeField]
        private GameNetworkRegistrySO _gameNetworkRegistrySo = null!;
        
        [SerializeField]
        private GameConfigs _gameConfigs = null!;
        
        [SerializeField]
        private Camera _mainCamera = null!;
        
        protected override void Configure(IContainerBuilder builder)
        {
            var netManager = FindFirstObjectByType<NetworkManager>();
            
            // Components
            builder.RegisterComponent(_startupNetwork);
            builder.RegisterComponent(_revisionAckNetwork);
            builder.RegisterInstance(_gameNetworkRegistrySo);
            builder.RegisterInstance(_mainCamera);
            
            // Singletons
            builder.Register<GameNetLoader>(Lifetime.Singleton).AsSelf();
            builder.Register<GameUILoader>(Lifetime.Singleton).AsSelf();
            builder.Register<PhasesHelper>(Lifetime.Singleton).AsSelf();
            builder.Register<GameCurrentPlayerInfoTabController>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
            builder.RegisterEntryPoint<GameServerTimeNetwork>();
            builder.Register<GameDataProviderNetwork>(Lifetime.Singleton).As<IGameDataProvider>();
            RegisterClientServices(builder);
            
            // Managers
            builder.Register<GameFieldManager>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<GameFieldViewManager>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<GamePlanetInfoPresenter>(Lifetime.Singleton).AsSelf();
            
            // Factories
            builder.Register<PlayerPlanetsFactory>(Lifetime.Singleton).AsSelf();
            builder.Register<GameShipsOnPlanetInfoViewFactory>(Lifetime.Singleton).AsSelf();
            builder.Register<GameShipsOnPlanetInfoViewModelFactory>(Lifetime.Singleton).AsSelf();
            
            // Creators
            builder.Register<GameFieldPlanetsViewProvider>(Lifetime.Singleton).AsSelf();
            
            // Configs
            _gameConfigs.Build(builder);
            
            // ViewModels
            RegisterViewModels(builder);
            
            // Depended network
            if (netManager.IsServer)
            {
                RegisterServerServices(builder);
            }
            
            // Entry Point
            builder.RegisterEntryPoint<GameStartupService>();
            builder.RegisterEntryPoint<GamePresentationStartup>();
        }
        
        private static void RegisterViewModels(IContainerBuilder builder)
        {
            RegisterViewModel<GameHudViewModel>(builder).AsImplementedInterfaces();
            RegisterViewModel<GameHudTopViewModel>(builder).AsImplementedInterfaces();
            RegisterViewModel<GameHudBottomViewModel>(builder).AsImplementedInterfaces();
            RegisterViewModel<GameHudBottomSwitchesViewModel>(builder);
            builder.RegisterEntryPoint<GamePlayerHandViewModel>();
            RegisterViewModel<GameArrowsHolderViewModel>(builder);
            RegisterViewModel<GameHudTimerPhaseViewModel>(builder).AsImplementedInterfaces();
        }

        private static RegistrationBuilder RegisterViewModel<T>(IContainerBuilder builder)
        {
            return builder.Register<T>(Lifetime.Transient);
        }

        private static void RegisterClientServices(IContainerBuilder builder)
        {
            // Unkeyed resolution always supplies the local client projection, including on the host.
            builder.Register<GameContext>(Lifetime.Singleton).WithParameter(GameContextRole.Client);
            builder.Register<PendingGameActions>(Lifetime.Singleton);
            builder.Register<GameEventBatchApplier>(Lifetime.Singleton);
            builder.Register<GameActionServerInteraction>(Lifetime.Singleton).As<IGameActionServerInteraction>();
            builder.Register<GameActionClient>(Lifetime.Singleton);
            builder.Register<GameSnapshotRecovery>(Lifetime.Singleton);
            builder.Register<GameClientSynchronizationService>(Lifetime.Singleton);
        }

        private static void RegisterServerServices(IContainerBuilder builder)
        {
            builder.Register<GameContext>(Lifetime.Singleton).Keyed(GameContextRole.Server).WithParameter(GameContextRole.Server);

            RegisterServerService<GameStateProjector>(builder);
            RegisterServerService<GameActionExecutor>(builder);
            RegisterServerService<GameServerBootstrap>(builder);
            RegisterServerService<GameServerRequestsRegisterService>(builder);
            RegisterServerService<ServerPhaseScheduler>(builder);

            builder.Register<GameEventBatchFactory>(Lifetime.Singleton);
            builder.Register<GameServerEventBroadcaster>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
            builder.Register<ServerClientRevisionTracker>(Lifetime.Singleton).AsSelf().As<IPhaseEntryAcknowledgements>();
            builder.RegisterInstance(new System.Random());
            builder.Register<ServerDestinyTargetSelector>(Lifetime.Singleton);

            builder.Register<CardsInitialStateFactory>(Lifetime.Singleton);
            builder.Register<GalaxyInitialStateFactory>(Lifetime.Singleton);
            builder.Register<PhaseInitialStateFactory>(Lifetime.Singleton);
            builder.Register<PlayerInitialStateFactory>(Lifetime.Singleton);
        }

        private static void RegisterServerService<T>(IContainerBuilder builder)
        {
            builder.Register<T>(Lifetime.Singleton).WithParameter<GameContext>(resolver => resolver.Resolve<GameContext>(GameContextRole.Server));
        }
    }
}
