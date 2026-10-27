using System;
using System.Collections.Generic;
using Network.Configs;
using VContainer;
using VContainer.Unity;

namespace Network.Game
{
    public sealed class GameNetLoader : IDisposable
    {
        private readonly IObjectResolver _objectResolver;
        private readonly GameNetworkRegistrySO _gameNetworkRegistrySO;
        private readonly Dictionary<ulong, IPrefabInitializerOnClients> _initializers = new();
        private bool _objectsCreated;

        public GameNetLoader(IObjectResolver objectResolver, GameNetworkRegistrySO gameNetworkRegistrySO)
        {
            _objectResolver = objectResolver;
            _gameNetworkRegistrySO = gameNetworkRegistrySO;
        }

        public event Action? NetworkObjectsLoadedOnAllClients;

        public bool IsLoaded { get; private set; }

        public void LoadNetGame()
        {
            LoadEventRpcRelayNetwork();
            _objectsCreated = true;
            TryCompleteLoading();
        }

        public void Dispose()
        {
            foreach (var initializer in _initializers.Values)
            {
                initializer.OnLoaded -= CheckGameComponentsLoaded;
            }

            _initializers.Clear();
        }

        private void LoadEventRpcRelayNetwork()
        {
            var eventRpcRelayPrefab = _gameNetworkRegistrySO.EventRpcRelayNetwork;
            var eventRpcRelayInstance = _objectResolver.Instantiate(eventRpcRelayPrefab, null);
            eventRpcRelayInstance.NetworkObject.Spawn(destroyWithScene: true);
            var eventBroadcaster = _objectResolver.Resolve<GameServerEventBroadcaster>();
            eventBroadcaster.SetRelayObject(eventRpcRelayInstance);
            AddToInitializer(eventRpcRelayInstance.NetworkObjectId, eventRpcRelayInstance.Initializer);
        }

        private void AddToInitializer(ulong networkObjectId, IPrefabInitializerOnClients initializer)
        {
            _initializers.Add(networkObjectId, initializer);
            initializer.OnLoaded += CheckGameComponentsLoaded;
            // On the host, readiness may have been reported synchronously inside Spawn.
            if (initializer.IsLoaded)
            {
                CheckGameComponentsLoaded(networkObjectId);
            }
        }

        private void CheckGameComponentsLoaded(ulong loadedPrefabId)
        {
            if (!_initializers.TryGetValue(loadedPrefabId, out var initializer))
            {
                return;
            }

            initializer.OnLoaded -= CheckGameComponentsLoaded;
            _initializers.Remove(loadedPrefabId);
            TryCompleteLoading();
        }

        private void TryCompleteLoading()
        {
            // Do not publish readiness before all objects have been created and the broadcaster is bound.
            if (IsLoaded || !_objectsCreated || _initializers.Count != 0)
            {
                return;
            }

            IsLoaded = true;
            NetworkObjectsLoadedOnAllClients?.Invoke();
        }
    }
}
