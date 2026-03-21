using System;
using System.Collections.Generic;
using System.Linq;
using Core.Player;
using Logs;
using Network.Configs;
using Unity.Netcode;
using VContainer;
using VContainer.Unity;

namespace Network.Game
{
    public class GameNetLoader : IDisposable
    {
        private readonly NetworkManager _networkManager;
        private readonly IObjectResolver _objectResolver;
        private readonly GameNetworkRegistrySO _gameNetworkRegistrySO;
        private readonly PlayersRegistry _playersRegistry;

        private GalaxyNetworkSync? _galaxyNetworkSync;
        private DestinyCardNetworkSync? _destinyCardNetworkSync;
        private readonly List<GamePlayerNetworkSync> _players = new();

        public GameNetLoader(
            NetworkManager networkManager,
            IObjectResolver objectResolver,
            GameNetworkRegistrySO gameNetworkRegistrySO,
            PlayersRegistry playersRegistry)
        {
            _networkManager = networkManager;
            _objectResolver = objectResolver;
            _gameNetworkRegistrySO = gameNetworkRegistrySO;
            _playersRegistry = playersRegistry;
        }

        public event Action? OnGameIsReady;

        public void LoadNetGame()
        {
            if (!_networkManager.IsServer)
            {
                Logger.Error("GameNetLoader.LoadNetGame: method available only on server.");
                
                return;
            }
            
            LoadGalaxyNetwork();
            LoadPlayersNetwork();
            LoadDestinyCardNetwork();
        }
        
        public void Dispose()
        {
            var notLoadedPlayers = _players.Where(p => !p.Initializer.IsLoaded);

            foreach (var player in notLoadedPlayers)
            {
                player.Initializer.OnLoaded -= HandleSinglePlayerLoaded;
            }

            if (_galaxyNetworkSync != null && !_galaxyNetworkSync.Initializer.IsLoaded)
            {
                _galaxyNetworkSync.Initializer.OnLoaded -= HandleGalaxyLoaded;
            }

            if (_destinyCardNetworkSync != null && !_destinyCardNetworkSync.Initializer.IsLoaded)
            {
                _destinyCardNetworkSync.Initializer.OnLoaded -= HandleDestinyCardLoaded;
            }
        }

        private void LoadGalaxyNetwork()
        {
            var galaxyPrefab = _gameNetworkRegistrySO.GalaxyNetworkSync;
            var galaxyInstance = _objectResolver.Instantiate(galaxyPrefab, null);
            galaxyInstance.NetworkObject.Spawn(destroyWithScene: true);
            _galaxyNetworkSync = galaxyInstance;
            galaxyInstance.Initializer.OnLoaded += HandleGalaxyLoaded;
        }

        private void LoadDestinyCardNetwork()
        {
            var destinyCardPrefab = _gameNetworkRegistrySO.DestinyCardNetworkSync;
            var destinyCardInstance = _objectResolver.Instantiate(destinyCardPrefab, null);
            destinyCardInstance.NetworkObject.Spawn(destroyWithScene: true);
            _destinyCardNetworkSync = destinyCardInstance;
            destinyCardInstance.Initializer.OnLoaded += HandleDestinyCardLoaded;
        }
        
        private void LoadPlayersNetwork()
        {
            var players = _playersRegistry.Players;

            foreach (var player in players)
            {
                var gamePlayerPrefab = _gameNetworkRegistrySO.GamePlayerNetworkSync;
                var gamePlayerInstance = _objectResolver.Instantiate(gamePlayerPrefab, null);
                gamePlayerInstance.NetworkObject.SpawnAsPlayerObject(player.ClientId, destroyWithScene: true);
                _players.Add(gamePlayerInstance);
                gamePlayerInstance.Initializer.OnLoaded += HandleSinglePlayerLoaded;
            }
        }

        private void HandleSinglePlayerLoaded(ulong loadedPrefabId)
        {
            var gamePlayer = _players.First(p => p.NetworkObjectId == loadedPrefabId);
            gamePlayer.Initializer.OnLoaded -= HandleSinglePlayerLoaded;
            CheckGameFullyLoaded();
        }

        private void HandleDestinyCardLoaded(ulong loadedPrefabId)
        {
            _destinyCardNetworkSync!.Initializer.OnLoaded -= HandleDestinyCardLoaded;
            CheckGameFullyLoaded();
        }
        
        private void HandleGalaxyLoaded(ulong loadedPrefabId)
        {
            _galaxyNetworkSync!.Initializer.OnLoaded -= HandleGalaxyLoaded;
            CheckGameFullyLoaded();
        }
        
        private void CheckGameFullyLoaded()
        {
            var areAllPlayersLoaded = _players.All(p => p.Initializer.IsLoaded);
            var isGalaxyLoaded = _galaxyNetworkSync != null && _galaxyNetworkSync.Initializer.IsLoaded;
            var isDestinyCardLoaded = _destinyCardNetworkSync != null;

            if (areAllPlayersLoaded && isGalaxyLoaded && isDestinyCardLoaded)
            {
                OnGameIsReady?.Invoke();
            }
        }
    }
}