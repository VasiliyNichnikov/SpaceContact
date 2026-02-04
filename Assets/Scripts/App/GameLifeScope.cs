using App.Factory;
using App.Services;
using Core;
using Core.Factory;
using Core.Phases;
using Network;
using Network.Infrastructure;
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

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(_networkManager);
            builder.RegisterComponent(_networkGameController);

            // Singletons
            builder.Register<PhaseRegistry>(Lifetime.Singleton).AsSelf();
            builder.Register<JsonNetworkSerializer>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<NetworkAutoInjector>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<PhaseRegistrationService>(Lifetime.Singleton).AsSelf();
            
            // Factories
            builder.Register<VContainerPhasesFactory>(Lifetime.Singleton).As<IPhaseFactory>();
            
            // Game State Machine
            builder.Register<GameStateMachine>(Lifetime.Singleton).AsSelf();
            
            // Phases
            RegisterPhases(builder);
            
            // Entry Point
            builder.RegisterEntryPoint<GameStartupService>();
        }

        private static void RegisterPhases(IContainerBuilder builder)
        {
            builder.Register<RegroupPhase>(Lifetime.Transient);
        }
    }
}