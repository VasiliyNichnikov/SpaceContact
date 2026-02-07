using System;
using System.Collections.Generic;
using Core.Player;
using Reactivity;

namespace Client.UI.Dialogs.Lobby
{
    public class LobbyViewModel : IDisposable
    {
        private readonly PlayersRegistry _playersRegistry;
        private readonly Dictionary<IPlayerManager, LobbyPlayerViewModel> _playerViewModelByManager;
        private readonly EventProvider _refreshPlayersEvent;
        private readonly IGameLevelControl _levelControl;
        private readonly ILobbyController _lobbyController;
        
        public LobbyViewModel(
            PlayersRegistry playersRegistry, 
            IGameLevelControl levelControl, 
            ILobbyController lobbyController)
        {
            _playersRegistry = playersRegistry;
            _levelControl = levelControl;
            _lobbyController = lobbyController;
            _playersRegistry.OnPlayerJoined += PlayerJoined;
            _playersRegistry.OnPlayerLeft += PlayerLeft;
            _playerViewModelByManager = CreatePlayers();
            _refreshPlayersEvent = new EventProvider();
        }

        public bool IsOwnerLobby => _lobbyController.IsOwnerLobby;
        
        public IReadOnlyCollection<LobbyPlayerViewModel> Players => _playerViewModelByManager.Values;

        public IEventProvider RefreshPlayersEvent => _refreshPlayersEvent;

        public void RunBattleButtonClickHandler() => 
            _levelControl.StartGame();
        
        public void Dispose()
        {
            _playersRegistry.OnPlayerJoined -= PlayerJoined;
            _playersRegistry.OnPlayerLeft -= PlayerLeft;

            foreach (var kvp in _playerViewModelByManager)
            {
                kvp.Value.Dispose();
            }
            
            _playerViewModelByManager.Clear();
        }

        private void PlayerJoined(IPlayerManager playerManager)
        {
            _playerViewModelByManager[playerManager] = new LobbyPlayerViewModel(playerManager);
            _refreshPlayersEvent.Call();
        }

        private void PlayerLeft(IPlayerManager playerManager)
        {
            _playerViewModelByManager[playerManager].Dispose();
            _playerViewModelByManager.Remove(playerManager);
            _refreshPlayersEvent.Call();
        }

        private Dictionary<IPlayerManager, LobbyPlayerViewModel> CreatePlayers()
        {
            var result = new Dictionary<IPlayerManager, LobbyPlayerViewModel>();
            
            foreach (var playerManager in _playersRegistry.Players)
            {
                result[playerManager] = new LobbyPlayerViewModel(playerManager);
            }
            
            return result;
        }
    }
}