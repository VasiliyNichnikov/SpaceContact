using Network.Configs;
using Unity.Netcode;
using VContainer;
using VContainer.Unity;

namespace Network.Game
{
    public class GameNetLoader
    {
        private readonly NetworkManager _networkManager;
        private readonly IObjectResolver _objectResolver;
        private readonly GameNetworkRegistrySO _gameNetworkRegistrySO;

        public GameNetLoader(
            NetworkManager networkManager,
            IObjectResolver objectResolver,
            GameNetworkRegistrySO gameNetworkRegistrySO)
        {
            _networkManager = networkManager;
            _objectResolver = objectResolver;
            _gameNetworkRegistrySO = gameNetworkRegistrySO;
        }

        public void LoadGalaxyNetwork()
        {
            if (!_networkManager.IsServer)
            {
                return;
            }

            var galaxyPrefab = _gameNetworkRegistrySO.GalaxyNetworkSync;
            var galaxyInstance = _objectResolver.Instantiate(galaxyPrefab, null);
            galaxyInstance.NetworkObject.Spawn(destroyWithScene: true);
        }
    }
}