using System;
using System.Collections.Generic;

namespace Core.Player
{
    public class PlayersRegistry
    {
        private readonly List<IPlayerManager> _players = new();

        public event Action<IPlayerManager>? OnPlayerJoined;
        
        public event Action<IPlayerManager>? OnPlayerLeft;
        
        public IEnumerable<IPlayerManager> Players => _players;
        
        public void Register(IPlayerManager player)
        {
            if (_players.Contains(player))
            {
                return;
            }
            
            _players.Add(player);
            OnPlayerJoined?.Invoke(player);
        }

        public void Unregister(IPlayerManager player)
        {
            if (_players.Remove(player))
            {
                OnPlayerLeft?.Invoke(player);
            }
        }
    }
}