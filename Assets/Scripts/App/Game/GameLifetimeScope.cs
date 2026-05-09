using App.Game.Factory;
using App.Game.Services;
using Client.Configs.Game;
using Client.Game;
using Client.Game.Factory;
using Client.Game.Field;
using Client.Game.Planets.ViewModels;
using Client.Helpers;
using Client.UI;
using Client.UI.Dialogs.Game.PlayerChoice.ViewModels;
using Client.UI.HUDs;
using Client.UI.HUDs.ViewModels;
using Client.UI.Loaders;
using Core.Game;
using Core.Game.Cards;
using Core.Game.Encounter;
using Core.Game.Factory;
using Core.Game.Galaxy;
using Core.Game.Galaxy.Server;
using Core.Game.Mutation;
using Core.Game.Phases;
using Core.Game.Phases.Client;
using Core.Game.Phases.Server;
using Core.Game.Planets;
using Core.Game.Players;
using Core.Game.Rules;
using Network;
using Network.Configs;
using Network.Game;
using Network.Game.Encounter;
using Network.Game.Hands;
using Network.Game.Mutation;
using Network.Game.Phases;
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
        
        [SerializeField]
        private Camera _mainCamera = null!;
        
        protected override void Configure(IContainerBuilder builder)
        {
            var netManager = FindFirstObjectByType<NetworkManager>();
            
            // Components
            builder.RegisterComponent(_networkGameController).AsImplementedInterfaces();
            builder.RegisterInstance(_gameNetworkRegistrySo);
            builder.RegisterInstance(_mainCamera);
            
            // Singletons
            builder.Register<PhaseRegistry>(Lifetime.Singleton).AsSelf();
            builder.Register<PhaseRegistrationService>(Lifetime.Singleton).AsSelf();
            builder.Register<GamePhaseController>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<GameNetLoader>(Lifetime.Singleton).AsSelf();
            builder.Register<GamePlayersRegistry>(Lifetime.Singleton).AsSelf();
            builder.Register<GamePlayersLoader>(Lifetime.Singleton).AsSelf();
            builder.Register<GameUILoader>(Lifetime.Singleton).AsSelf();
            builder.Register<GamePlayersPhaseTracker>(Lifetime.Singleton).AsSelf();
            builder.Register<PhasesHelper>(Lifetime.Singleton).AsSelf();
            builder.Register<GameDestinyTargetSelector>(Lifetime.Singleton).AsSelf();
            builder.Register<GamePlanetAttackTargetSelector>(Lifetime.Singleton).AsSelf();
            builder.Register<GameCurrentPlayerInfoTabController>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
            builder.Register<GamePhaseTimeController>(Lifetime.Singleton).AsSelf();
            builder.Register<GameServerTimeNetwork>(Lifetime.Singleton).AsImplementedInterfaces();
            
            // Managers
            builder.Register<GameFieldManager>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<GameFieldViewManager>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<GamePlanetInfoPresenter>(Lifetime.Singleton).AsSelf();
            
            // Game State Machine
            builder.Register<GameStateMachine>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            
            // Factories
            builder.Register<VContainerPhasesFactory>(Lifetime.Singleton).As<IPhaseFactory>();
            builder.Register<PlayerPlanetsFactory>(Lifetime.Singleton).AsSelf();
            builder.Register<SpaceCardFactory>(Lifetime.Singleton).AsSelf();
            builder.Register<DestinyCardFactory>(Lifetime.Singleton).AsSelf();
            builder.Register<GameShipsOnPlanetInfoViewFactory>(Lifetime.Singleton).AsSelf();
            builder.Register<GameEventsFactory>(Lifetime.Singleton).AsSelf();
            builder.Register<GameShipsOnPlanetInfoViewModelFactory>(Lifetime.Singleton).AsSelf();
            
            // Creators
            builder.Register<GameFieldPlanetsViewProvider>(Lifetime.Singleton).AsSelf();
            
            // Server Interactions
            builder.Register<GamePhaseServerInteraction>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<GamePlayerHandServerInteraction>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<GameEncounterServerInteraction>(Lifetime.Singleton).AsImplementedInterfaces();
            
            // Phases
            RegisterPhases(builder, netManager.IsServer);
            
            // Configs
            _gameConfigs.Build(builder);
            
            // ViewModels
            RegisterViewModels(builder);
            
            // Depended network
            RegisterElementsDependedNetwork(builder, netManager.IsServer);
            
            // Register Game Rules
            RegisterGameRules(builder);
            
            // Entry Point
            builder.RegisterEntryPoint<GameStartupService>();
        }
        
        private static void RegisterPhases(IContainerBuilder builder, bool isServer)
        {
            builder.Register<GameInitializationPhase>(
                resolver => GamePhaseFactory.CreateInitializationPhase(resolver, isServer), 
                Lifetime.Transient);

            builder.Register<GameFirstMovePhase>(
                resolver => GamePhaseFactory.CreateFirstMovePhase(resolver, isServer),
                Lifetime.Transient);
            
            builder.Register<GameRegroupPhase>(Lifetime.Transient);
            
            builder.Register<GameDestinyPhase>(
                resolver => GamePhaseFactory.CreateDestinyPhase(resolver, isServer), 
                Lifetime.Transient);
        }
        
        private static void RegisterViewModels(IContainerBuilder builder)
        {
            RegisterViewModel<GameHudViewModel>(builder).AsImplementedInterfaces();
            RegisterViewModel<GameHudTopViewModel>(builder).AsImplementedInterfaces();
            RegisterViewModel<GameHudBottomViewModel>(builder).AsImplementedInterfaces();
            RegisterViewModel<GameArrowsHolderViewModel>(builder);
            RegisterViewModel<GameHudTimerPhaseViewModel>(builder).AsImplementedInterfaces();
        }

        private static RegistrationBuilder RegisterViewModel<T>(IContainerBuilder builder)
        {
            return builder.Register<T>(Lifetime.Transient);
        }

        private static void RegisterElementsDependedNetwork(IContainerBuilder builder, bool isServer)
        {
            builder.Register<GameClientDestinyPhaseResolver>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<GameClientEncounterManager>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<GameClientGalaxyManager>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<GameClientPlayerCardsDeckService>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<GameClientEventContext>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
            builder.Register<GameClientPlayerReadinessController>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
            
            if (isServer)
            {
                builder.Register<GameServerRequestsRegisterService>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
                builder.Register<GameServerDestinyPhaseResolver>(Lifetime.Singleton).AsImplementedInterfaces();
                builder.Register<GameServerEncounterManager>(Lifetime.Singleton).AsImplementedInterfaces();
                builder.Register<GameServerEventBroadcaster>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
                builder.Register<GameServerSimpleEncounterState>(Lifetime.Singleton).AsSelf();
                builder.Register<GameServerGalaxyManager>(Lifetime.Singleton).AsImplementedInterfaces();
                builder.Register<GameServerCoreLoader>(Lifetime.Singleton).AsSelf();
                builder.Register<GameServerCardsManager>(Lifetime.Singleton).AsImplementedInterfaces();
                builder.Register<GameServerPhasePayloadFactory>(Lifetime.Singleton).AsSelf();
                builder.Register<GameServerPhaseTransitioner>(Lifetime.Singleton).AsSelf();
                builder.Register<GameServerChangerStatePlayersReadiness>(Lifetime.Singleton).AsSelf();
            }
        }
        
        private static void RegisterGameRules(IContainerBuilder builder)
        {
            builder.Register<GameRulesChecker>(Lifetime.Singleton);
            
            RegisterGameRule<GameCanBeAggressorRule>(builder);
            RegisterGameRule<GameCanBeDefenderRule>(builder);
            RegisterGameRule<GameCanApplyDestinyCardRule>(builder);
            RegisterGameRule<GameCanSkipDestinyCardRule>(builder);
            RegisterGameRule<GameCanAggressorAttackToPlanetRule>(builder);
            RegisterGameRule<GameCanChoosePlanetToAttackRule>(builder);
            RegisterGameRule<GameCanPlayerChangeToReadyRule>(builder);
            RegisterGameRule<GameCanPlayerChangeToNotReadyRule>(builder);
        }
        
        private static void RegisterGameRule<TRule>(IContainerBuilder builder) where TRule : IGameRule
        {
            builder.Register<TRule>(Lifetime.Singleton).As<IGameRule>();
        }
    }
}