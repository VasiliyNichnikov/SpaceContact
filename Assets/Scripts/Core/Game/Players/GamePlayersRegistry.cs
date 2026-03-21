using System.Collections.Generic;
using Logs;

namespace Core.Game.Players
{
    public class GamePlayersRegistry
    {
        private readonly Dictionary<ulong, IGamePlayer> _playerByClientId = new();

        public IReadOnlyCollection<IGamePlayer> Players => 
            _playerByClientId.Values;

        private ulong? _ownerPlayerId;

        public IGamePlayer? GetOwnerWithError()
        {
            if (_ownerPlayerId == null)
            {
                Logger.Error("GamePlayersRegistry.GetOwnerWithError: ownerPlayerId is null.");
                return null;
            }

            if (_playerByClientId.TryGetValue(_ownerPlayerId.Value, out var player))
            {
                return player;
            }
            
            Logger.Error($"GamePlayersRegistry.GetOwnerWithError: player with id {_ownerPlayerId} not found.");
            
            return null;
        }
        
        public IGamePlayer GetPlayerById(ulong playerId)
        {
            if (_playerByClientId.TryGetValue(playerId, out var player))
            {
                return player;
            }
            
            Logger.Error($"GamePlayersRegistry.GetPlayerById: player with id {playerId} not found.");
            
            return EmptyGamePlayer.Instance;
        }

        public void AddPlayer(IGamePlayer player, bool isOwner)
        {
            if (isOwner)
            {
                if (_ownerPlayerId != null)
                {
                    Logger.Error("GamePlayersRegistry.AddPlayer: there can't be two owners");
                }
                
                _ownerPlayerId = player.PlayerId;
            }
            
            _playerByClientId.Add(player.PlayerId, player);
        }
    }
}