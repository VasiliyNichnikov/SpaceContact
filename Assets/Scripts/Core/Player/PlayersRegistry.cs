using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Player
{
    public class PlayersRegistry
    {
        private readonly List<IPlayerManager> _players = new();

        public event Action<IPlayerManager>? OnPlayerJoined;
        
        public event Action<IPlayerManager>? OnPlayerLeft;
        
        public IEnumerable<IPlayerManager> Players => _players;
        
        public IEnumerable<IPlayerManager> OtherPlayers => _players
            .Where(player => !player.IsCurrentPlayer);

        public IPlayerManager? CurrentPlayer;
        
        public void Register(IPlayerManager player)
        {
            if (_players.Contains(player))
            {
                return;
            }

            if (player.IsCurrentPlayer)
            {
                CurrentPlayer = player;
            }
            
            _players.Add(player);
            OnPlayerJoined?.Invoke(player);
        }

        public void Unregister(IPlayerManager player)
        {
            if (!_players.Remove(player))
            {
                return;
            }
            
            if (player.IsCurrentPlayer)
            {
                CurrentPlayer = null;
            }
                
            OnPlayerLeft?.Invoke(player);
        }
    }
}