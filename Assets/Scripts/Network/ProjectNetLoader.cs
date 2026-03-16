using Logs;
using Network.Configs;
using Unity.Netcode;
using VContainer;
using VContainer.Unity;

namespace Network
{
    public class ProjectNetLoader
    {
        private readonly NetworkManager _networkManager;
        private readonly IObjectResolver _resolver;
        private readonly ProjectNetworkRegistrySO _projectNetworkRegistrySO;
        
        public ProjectNetLoader(
            NetworkManager networkManager,
            IObjectResolver resolver,
            ProjectNetworkRegistrySO projectNetworkRegistrySO)
        {
            _networkManager = networkManager;
            _resolver = resolver;
            _projectNetworkRegistrySO = projectNetworkRegistrySO;
        }

        public void LoadNetProject()
        {
            if (!_networkManager.IsServer)
            {
                Logger.Error("ProjectNetLoader.LoadNetProject: method available only on server.");
                
                return;
            }

            LoadServiceObj();
        }

        private void LoadServiceObj()
        {
            var networkServiceObjPrefab = _projectNetworkRegistrySO.NetworkServiceObj;
            var serviceObjInstance = _resolver.Instantiate(networkServiceObjPrefab, null);
            serviceObjInstance.NetworkObject.Spawn(destroyWithScene: false);
        }
    }
}