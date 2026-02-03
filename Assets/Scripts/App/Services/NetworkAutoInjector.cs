using Unity.Netcode;
using VContainer;
using VContainer.Unity;

namespace App.Services
{
    public class NetworkAutoInjector : IStartable
    {
        private readonly IObjectResolver _resolver;
        private readonly NetworkManager _networkManager;

        public NetworkAutoInjector(IObjectResolver resolver, NetworkManager networkManager)
        {
            _resolver = resolver;
            _networkManager = networkManager;
        }
        
        public void Start()
        {
            foreach (var networkPrefab in _networkManager.NetworkConfig.Prefabs.Prefabs)
            {
                var prefab = networkPrefab.Prefab;
                var prefabHandler = new VContainerNetworkInterceptor(_resolver, prefab);
                _networkManager.PrefabHandler.AddHandler(prefab, prefabHandler);
            }
        }
    }
}