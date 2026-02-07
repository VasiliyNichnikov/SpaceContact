using System;
using System.Collections.Generic;
using Network.Player;
using Reactivity;
using Unity.Netcode;

namespace Client.UI.Dialogs.Lobby
{
    public class LobbyViewModel : IDisposable
    {
        private readonly PlayersRegistry _playersRegistry;
        private readonly Dictionary<PlayerNetworkState, LobbyPlayerViewModel> _playerViewModelByState;
        private readonly EventProvider _refreshPlayersEvent;
        private readonly IGameLevelControl _levelControl;
        private readonly NetworkManager _networkManager;
        
        public LobbyViewModel(
            PlayersRegistry playersRegistry, 
            IGameLevelControl levelControl, 
            NetworkManager networkManager)
        {
            _playersRegistry = playersRegistry;
            _levelControl = levelControl;
            _networkManager = networkManager;
            _playersRegistry.OnPlayerJoined += PlayerJoined;
            _playersRegistry.OnPlayerLeft += PlayerLeft;
            _playerViewModelByState = CreatePlayers();
            _refreshPlayersEvent = new EventProvider();
        }

        public bool IsOwnerLobby => _networkManager.LocalClientId == NetworkManager.ServerClientId;
        
        public IReadOnlyCollection<LobbyPlayerViewModel> Players => _playerViewModelByState.Values;

        public IEventProvider RefreshPlayersEvent => _refreshPlayersEvent;

        public void RunBattleButtonClickHandler() => 
            _levelControl.StartGame();
        
        public void Dispose()
        {
            _playersRegistry.OnPlayerJoined -= PlayerJoined;
            _playersRegistry.OnPlayerLeft -= PlayerLeft;

            foreach (var kvp in _playerViewModelByState)
            {
                kvp.Value.Dispose();
            }
            
            _playerViewModelByState.Clear();
        }

        private void PlayerJoined(PlayerNetworkState state)
        {
            _playerViewModelByState[state] = CreatePlayerViewModelFromState(state);
            _refreshPlayersEvent.Call();
        }

        private void PlayerLeft(PlayerNetworkState state)
        {
            _playerViewModelByState[state].Dispose();
            _playerViewModelByState.Remove(state);
            _refreshPlayersEvent.Call();
        }

        private Dictionary<PlayerNetworkState, LobbyPlayerViewModel> CreatePlayers()
        {
            var result = new Dictionary<PlayerNetworkState, LobbyPlayerViewModel>();
            
            foreach (var state in _playersRegistry.GetAllPlayers())
            {
                result[state] = CreatePlayerViewModelFromState(state);
            }
            
            return result;
        }
        
        private static LobbyPlayerViewModel CreatePlayerViewModelFromState(PlayerNetworkState state)
        {
            return new LobbyPlayerViewModel(state.CorePlayer);
        }
    }
}