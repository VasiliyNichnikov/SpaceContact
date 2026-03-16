using App.Game.Factory;
using App.Game.Services;
using Client.Configs.Game;
using Client.Game;
using Client.Game.Factory;
using Client.Game.Field;
using Core.Game;
using Core.Game.Cards;
using Core.Game.Factory;
using Core.Game.Galaxy;
using Core.Game.Phases;
using Core.Game.Players;
using Network.Configs;
using Network.Game;
using Network.Infrastructure;
using UnityEngine;
using VContainer;
using VContainer.Unity;

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
            // Components
            builder.RegisterComponent(_networkGameController);
            builder.RegisterInstance(_gameNetworkRegistrySo);
            
            // Singletons
            builder.Register<PhaseRegistry>(Lifetime.Singleton).AsSelf();
            builder.Register<PhaseRegistrationService>(Lifetime.Singleton).AsSelf();
            builder.Register<GamePhaseController>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<GameNetLoader>(Lifetime.Singleton).AsSelf();
            builder.Register<GamePlayersRegistry>(Lifetime.Singleton).AsSelf();
            builder.Register<GameServerCoreLoader>(Lifetime.Singleton).AsSelf();
            builder.Register<GamePlayersLoader>(Lifetime.Singleton).AsSelf();
            builder.Register<GameRequestsRegisterService>(Lifetime.Singleton).AsSelf();
            
            // Managers
            builder.Register<GameGalaxyManager>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<GameFieldManager>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<GameCardsManager>(Lifetime.Singleton).AsImplementedInterfaces();
            
            // Game State Machine
            builder.Register<GameStateMachine>(Lifetime.Singleton).AsSelf();
            
            // Factories
            builder.Register<VContainerPhasesFactory>(Lifetime.Singleton).As<IPhaseFactory>();
            builder.Register<PlayerPlanetsFactory>(Lifetime.Singleton).AsSelf();
            
            // Creators
            builder.Register<FieldObjectsCreator>(Lifetime.Singleton).AsSelf();
            
            // Phases
            RegisterPhases(builder);
            
            // Configs
            _gameConfigs.Build(builder);
            
            // Entry Point
            builder.RegisterEntryPoint<GameStartupService>();
        }
        
        private static void RegisterPhases(IContainerBuilder builder)
        {
            builder.Register<GameInitializationPhase>(Lifetime.Transient);
        }
    }
}