using System;
using System.Collections.Generic;
using Core.Game.Dto.Snapshots;

namespace Core.Game
{
    public sealed class PlayersContext : IContext<GamePlayersSnapshot>
    {
        private Dictionary<ulong, GamePlayerData> _players = new();
        
        public event Action<ulong>? OnReadinessPlayerChanged;
        
        public IReadOnlyCollection<GamePlayerData> All => 
            _players.Values;
        
        public void SetReadyState(ulong playerId, bool isReady)
        {
            var player = Get(playerId);
            
            if (player.TryChangeIsReadyValue(isReady))
            {
                OnReadinessPlayerChanged?.Invoke(playerId);
            }
        }

        public GamePlayerData Get(ulong playerId) => 
            _players[playerId];

        public bool IsPlayerExist(ulong playerId) => 
            _players.ContainsKey(playerId);

        public void LoadSnapshot(GamePlayersSnapshot snapshot)
        {
            var players = new Dictionary<ulong, GamePlayerData>();

            foreach (var playerSnapshot in snapshot.Players)
            {
                players.Add(playerSnapshot.PlayerId, GamePlayerData.Convert(playerSnapshot));
            }

            _players = players;
        }

        public GamePlayersSnapshot ToSnapshot()
        {
            var snapshot = new GamePlayersSnapshot();

            foreach (var (playerId, playerData) in _players)
            {
                var createdPlayerSnapshot = new GamePlayerSnapshot
                {
                    PlayerId = playerId,
                    Name = playerData.Name,
                    Order = playerData.Order,
                    Color = playerData.Color,
                    IsReady = playerData.IsReady,
                };
                snapshot.Players.Add(createdPlayerSnapshot);
            }
            
            return snapshot;
        }

        internal void ReplaceState(PlayersContext prepared)
        {
            _players = prepared._players;
        }
    }
}
