using System;
using System.Collections.Generic;
using Core.User;
using Logs;
using Network.Configs;
using Network.Game.Mutation;
using Unity.Netcode;
using VContainer;
using VContainer.Unity;

namespace Network.Game
{
    public sealed class GameNetLoader : IDisposable
    {
        private readonly NetworkManager _networkManager;
        private readonly IObjectResolver _objectResolver;
        private readonly GameNetworkRegistrySO _gameNetworkRegistrySO;
        private readonly ClientUsersRepository _usersRepository;
        private readonly Dictionary<ulong, IPrefabInitializerOnClients?> _initializers = new();
        
        public GameNetLoader(
            NetworkManager networkManager,
            IObjectResolver objectResolver,
            GameNetworkRegistrySO gameNetworkRegistrySO,
            ClientUsersRepository usersRepository)
        {
            _networkManager = networkManager;
            _objectResolver = objectResolver;
            _gameNetworkRegistrySO = gameNetworkRegistrySO;
            _usersRepository = usersRepository;
        }

        public event Action? OnGameIsReady;

        public void LoadNetGame()
        {
            if (!_networkManager.IsServer)
            {
                Logger.Error("GameNetLoader.LoadNetGame: method available only on server.");
                
                return;
            }
            
            LoadPlayersNetwork();
            LoadEventRpcRelayNetwork();
        }
        
        public void Dispose()
        {
            if (_initializers.Count == 0)
            {
                return;
            }

            foreach (var initializer in _initializers.Values)
            {
                if (initializer == null)
                {
                    continue;
                }

                initializer.OnLoaded -= CheckGameComponentsLoaded;
            }
            
            _initializers.Clear();
        }

        private void LoadEventRpcRelayNetwork()
        {
            var eventRpcRelayPrefab = _gameNetworkRegistrySO.EventRpcRelayNetwork;
            var eventRpcRelayInstance = _objectResolver.Instantiate(eventRpcRelayPrefab, null);
            eventRpcRelayInstance.NetworkObject.Spawn(destroyWithScene: true);
            AddToInitializer(eventRpcRelayInstance.NetworkObjectId, eventRpcRelayInstance.Initializer);
            
            var eventBroadcaster = _objectResolver.Resolve<GameServerEventBroadcaster>();
            eventBroadcaster.Bind(eventRpcRelayInstance);
        }
        
        private void LoadPlayersNetwork()
        {
            var users = _usersRepository.Users;

            foreach (var player in users)
            {
                var gamePlayerPrefab = _gameNetworkRegistrySO.PlayerNetworkSync;
                var gamePlayerInstance = _objectResolver.Instantiate(gamePlayerPrefab, null);
                gamePlayerInstance.NetworkObject.SpawnAsPlayerObject(player.ClientId, destroyWithScene: true);
                AddToInitializer(gamePlayerInstance.NetworkObjectId, gamePlayerInstance.Initializer);
            }
        }
        
        private void AddToInitializer(ulong networkObjectId, IPrefabInitializerOnClients initializer)
        {
            if (_initializers.ContainsKey(networkObjectId))
            {
                Logger.Error($"{nameof(GameNetLoader)}.{nameof(AddToInitializer)}: already initializer for {networkObjectId}.");
                return;
            }
            
            initializer.OnLoaded += CheckGameComponentsLoaded;
            _initializers.Add(networkObjectId, initializer);
        }

        private void CheckGameComponentsLoaded(ulong loadedPrefabId)
        {
            if (_initializers.TryGetValue(loadedPrefabId, out var initializer) && initializer != null)
            {
                initializer.OnLoaded -= CheckGameComponentsLoaded;
                _initializers.Remove(loadedPrefabId);
            }
            else
            {
                Logger.Error($"{nameof(GameNetLoader)}.{nameof(CheckGameComponentsLoaded)}: initializer for {loadedPrefabId} not found.");
            }
            
            if (_initializers.Count == 0)
            {
                OnGameIsReady?.Invoke();
            }
        }
    }
}