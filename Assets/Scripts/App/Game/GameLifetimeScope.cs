using App.Game.Factory;
using App.Game.Services;
using Core.Game.Factory;
using Core.Game.Phases;
using Network;
using Network.Game;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace App.Game
{
    public class GameLifetimeScope : LifetimeScope
    {
        [SerializeField]
        private NetworkGameController _networkGameController = null!;
        
        protected override void Configure(IContainerBuilder builder)
        {
            // Components
            builder.RegisterComponent(_networkGameController);
            
            // Singletons
            builder.Register<PhaseRegistrationService>(Lifetime.Singleton).AsSelf();
            
            // Factories
            builder.Register<VContainerPhasesFactory>(Lifetime.Singleton).As<IPhaseFactory>();
            
            // Phases
            RegisterPhases(builder);
            
            // Entry Point
            builder.RegisterEntryPoint<GameStartupService>();
        }
        
        private static void RegisterPhases(IContainerBuilder builder)
        {
            builder.Register<GameInitializationPhase>(Lifetime.Transient);
        }
    }
}