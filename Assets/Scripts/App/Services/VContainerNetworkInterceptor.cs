using ServiceLayer;
using Unity.Netcode;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace App.Services
{
    public class VContainerNetworkInterceptor : INetworkPrefabInstanceHandler
    {
        private readonly LifetimeScope _parentScope;
        private readonly IObjectResolver _mainResolver;
        private readonly GameObject _prefab;
        private readonly ContainerRegistrationService _containerRegistrationService;
        
        public VContainerNetworkInterceptor(
            LifetimeScope parentScope,
            IObjectResolver mainResolver, 
            GameObject prefab,
            ContainerRegistrationService containerRegistrationService)
        {
            _parentScope = parentScope;
            _mainResolver = mainResolver;
            _prefab = prefab;
            _containerRegistrationService = containerRegistrationService;
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
                instance = CreateInstanceAndInject(position, rotation);
            }
            
            return instance.GetComponent<NetworkObject>();
        }

        public void Destroy(NetworkObject networkObject)
        {
            Object.Destroy(networkObject.gameObject);
        }

        private GameObject CreateInstanceAndInject(Vector3 position, Quaternion rotation)
        {
            var instance = Object.Instantiate(_prefab, position, rotation);

            if (instance.TryGetComponent<TargetContainer>(out var target) && 
                _containerRegistrationService.TryGetResolver(target.ContainerType, out var resolver))
            {
                resolver.InjectGameObject(instance);

                return instance;
            }
            
            _mainResolver.InjectGameObject(instance);

            return instance;
        }
    }
}