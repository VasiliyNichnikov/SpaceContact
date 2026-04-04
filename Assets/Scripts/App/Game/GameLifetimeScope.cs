using App.Game.Factory;
using App.Game.Services;
using Client.Configs.Game;
using Client.Game;
using Client.Game.Factory;
using Client.Game.Field;
using Client.Helpers;
using Client.UI.Dialogs.Game.PlayerChoice.ViewModels;
using Client.UI.HUDs.ViewModels;
using Client.UI.Loaders;
using Core.Game;
using Core.Game.Cards;
using Core.Game.Factory;
using Core.Game.Galaxy;
using Core.Game.Phases;
using Core.Game.Players;
using Network.Configs;
using Network.Game;
using Network.Infrastructure;
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
        private NetworkGameController _networkGameController = null!;
        
        [SerializeField]
        private GameNetworkRegistrySO _gameNetworkRegistrySo = null!;
        
        [SerializeField]
        private GameConfigs _gameConfigs = null!;
        
        protected override void Configure(IContainerBuilder builder)
        {
            var netManager = FindFirstObjectByType<NetworkManager>();
            
            // Components
            builder.RegisterComponent(_networkGameController).AsImplementedInterfaces();
            builder.RegisterInstance(_gameNetworkRegistrySo);
            
            // Singletons
            builder.Register<PhaseRegistry>(Lifetime.Singleton).AsSelf();
            builder.Register<PhaseRegistrationService>(Lifetime.Singleton).AsSelf();
            builder.Register<GamePhaseController>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<GameNetLoader>(Lifetime.Singleton).AsSelf();
            builder.Register<GamePlayersRegistry>(Lifetime.Singleton).AsSelf();
            builder.Register<GameServerCoreLoader>(Lifetime.Singleton).AsSelf();
            builder.Register<GamePlayersLoader>(Lifetime.Singleton).AsSelf();
            builder.Register<GameUILoader>(Lifetime.Singleton).AsSelf();
            builder.Register<GameRequestsRegisterService>(Lifetime.Singleton).AsSelf();
            builder.Register<GamePlayersPhaseTracker>(Lifetime.Singleton).AsSelf();
            builder.Register<PhasesHelper>(Lifetime.Singleton).AsSelf();
            
            // Managers
            builder.Register<GameGalaxyManager>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<GameFieldManager>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<GameCardsManager>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<GameFieldViewManager>(Lifetime.Singleton).AsImplementedInterfaces();
            
            // Game State Machine
            builder.Register<GameStateMachine>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            
            // Factories
            builder.Register<VContainerPhasesFactory>(Lifetime.Singleton).As<IPhaseFactory>();
            builder.Register<PlayerPlanetsFactory>(Lifetime.Singleton).AsSelf();
            builder.Register<SpaceCardFactory>(Lifetime.Singleton).AsSelf();
            builder.Register<DestinyCardFactory>(Lifetime.Singleton).AsSelf();
            
            // Creators
            builder.Register<GameFieldPlanetsViewProvider>(Lifetime.Singleton).AsSelf();
            
            // Phases
            RegisterPhases(builder, netManager.IsServer);
            
            // Configs
            _gameConfigs.Build(builder);
            
            // ViewModels
            RegisterViewModels(builder);
            
            // Depended network
            RegisterElementsDependedNetwork(builder, netManager.IsServer);
            
            // Entry Point
            builder.RegisterEntryPoint<GameStartupService>();
        }
        
        private static void RegisterPhases(IContainerBuilder builder, bool isServer)
        {
            builder.Register<GameInitializationPhase>(
                resolver => GamePhaseFactory.CreateInitializationPhase(resolver, isServer), 
                Lifetime.Transient);
            
            builder.Register<GameDestinyPhase>(
                resolver => GamePhaseFactory.CreateDestinyPhase(resolver, isServer), 
                Lifetime.Transient);
        }
        
        private static void RegisterViewModels(IContainerBuilder builder)
        {
            RegisterViewModel<GameHudViewModel>(builder).AsImplementedInterfaces();
            RegisterViewModel<GameHudTopViewModel>(builder).AsImplementedInterfaces();
            RegisterViewModel<GameArrowsHolderViewModel>(builder);
        }

        private static RegistrationBuilder RegisterViewModel<T>(IContainerBuilder builder)
        {
            return builder.Register<T>(Lifetime.Transient);
        }

        private static void RegisterElementsDependedNetwork(IContainerBuilder builder, bool isServer)
        {
            builder.Register<GameClientDestinyCardController>(Lifetime.Singleton).AsImplementedInterfaces();
            
            if (isServer)
            {
                builder.Register<GameServerDestinyCardController>(Lifetime.Singleton).AsImplementedInterfaces();
            }
        }
    }
}