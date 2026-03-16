using System.Collections.Generic;
using Logs;

namespace Core.Game.Players
{
    public class GamePlayersRegistry
    {
        private readonly Dictionary<ulong, IGamePlayer> _playerByClientId = new();

        public IReadOnlyCollection<IGamePlayer> Players => 
            _playerByClientId.Values;
        
        public IGamePlayer GetPlayerById(ulong playerId)
        {
            if (_playerByClientId.TryGetValue(playerId, out var player))
            {
                return player;
            }
            
            Logger.Error($"GamePlayersRegistry.GetPlayerById: player with id {playerId} not found.");
            
            return EmptyGamePlayer.Instance;
        }

        public void AddPlayer(IGamePlayer player)
        {
            _playerByClientId.Add(player.PlayerId, player);
        }
    }
}