using ServiceLayer;
using Unity.Netcode;
using VContainer;
using VContainer.Unity;

namespace App.Services
{
    public class NetworkAutoInjector : IStartable
    {
        private readonly LifetimeScope _parentScope;
        private readonly IObjectResolver _resolver;
        private readonly NetworkManager _networkManager;
        private readonly ContainerRegistrationService _containerRegistrationService;

        public NetworkAutoInjector(
            LifetimeScope parentScope, 
            IObjectResolver resolver, 
            NetworkManager networkManager,
            ContainerRegistrationService containerRegistrationService)
        {
            _parentScope = parentScope;
            _resolver = resolver;
            _networkManager = networkManager;
            _containerRegistrationService = containerRegistrationService;
        }
        
        public void Start()
        {
            foreach (var networkPrefab in _networkManager.NetworkConfig.Prefabs.Prefabs)
            {
                var prefab = networkPrefab.Prefab;
                var prefabHandler = new VContainerNetworkInterceptor(
                    _parentScope, 
                    _resolver, 
                    prefab,
                    _containerRegistrationService);
                _networkManager.PrefabHandler.AddHandler(prefab, prefabHandler);
            }
        }
    }
}