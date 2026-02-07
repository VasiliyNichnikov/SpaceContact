using System;
using System.Collections.Generic;

namespace Network.Player
{
    public class PlayersRegistry
    {
        private readonly List<PlayerNetworkState> _players = new();

        public event Action<PlayerNetworkState>? OnPlayerJoined;
        
        public event Action<PlayerNetworkState>? OnPlayerLeft;
        
        public IEnumerable<PlayerNetworkState> GetAllPlayers() => 
            _players;

        public void Register(PlayerNetworkState player)
        {
            if (_players.Contains(player))
            {
                return;
            }
            
            _players.Add(player);
            OnPlayerJoined?.Invoke(player);
        }

        public void Unregister(PlayerNetworkState player)
        {
            if (_players.Remove(player))
            {
                OnPlayerLeft?.Invoke(player);
            }
        }
    }
}