using App.Services;
using Core;
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

        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<JsonNetworkSerializer>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.RegisterComponent(_networkManager);
            builder.Register<GameStateMachine>(Lifetime.Singleton);
            builder.Register<NetworkAutoInjector>(Lifetime.Singleton).AsImplementedInterfaces();
        }
    }
}