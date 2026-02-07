using Unity.Netcode;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace App.Services
{
    public class VContainerNetworkInterceptor : INetworkPrefabInstanceHandler
    {
        private readonly LifetimeScope _parentScope;
        private readonly IObjectResolver _resolver;
        private readonly GameObject _prefab;
        
        public VContainerNetworkInterceptor(
            LifetimeScope parentScope,
            IObjectResolver resolver, 
            GameObject prefab)
        {
            _parentScope = parentScope;
            _resolver = resolver;
            _prefab = prefab;
        }
        
        public NetworkObject Instantiate(ulong ownerClientId, Vector3 position, Quaternion rotation)
        {
            GameObject instance;
            
            if (_prefab.GetComponent<LifetimeScope>() != null)
            {
                using (LifetimeScope.EnqueueParent(_parentScope))
                {
                    instance = Object.Instantiate(_prefab, position, rotation);
                }
            }
            else
            {
                instance = Object.Instantiate(_prefab, position, rotation);
                _resolver.InjectGameObject(instance);
            }
            
            return instance.GetComponent<NetworkObject>();
        }

        public void Destroy(NetworkObject networkObject)
        {
            Object.Destroy(networkObject.gameObject);
        }
    }
}