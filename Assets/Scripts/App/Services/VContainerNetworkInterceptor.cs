using Unity.Netcode;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace App.Services
{
    public class VContainerNetworkInterceptor : INetworkPrefabInstanceHandler
    {
        private readonly IObjectResolver _resolver;
        private readonly GameObject _prefab;
        
        public VContainerNetworkInterceptor(IObjectResolver resolver, GameObject prefab)
        {
            _resolver = resolver;
            _prefab = prefab;
        }
        
        public NetworkObject Instantiate(ulong ownerClientId, Vector3 position, Quaternion rotation)
        {
            var instance = Object.Instantiate(_prefab, position, rotation);
            
            _resolver.InjectGameObject(instance);
            
            return instance.GetComponent<NetworkObject>();
        }

        public void Destroy(NetworkObject networkObject)
        {
            Object.Destroy(networkObject.gameObject);
        }
    }
}