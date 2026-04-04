using System.Collections.Generic;
using System.Linq;
using Core.Game.Players;

namespace Core.Game
{
    public sealed class GameFieldManager : IGameFieldManager
    {
        /// <summary>
        /// В будущем будет настраиваться во время запуска игры
        /// </summary>
        private const int NumberPlanetsOnPlayer = 5;
        
        private readonly GamePlayersRegistry _playersRegistry;
        
        public GameFieldManager(GamePlayersRegistry playersRegistry)
        {
            _playersRegistry = playersRegistry;
        }

        public IGamePlayer CurrentPlayer => 
            _playersRegistry.GetOwnerWithError() ?? EmptyGamePlayer.Instance;

        public IReadOnlyCollection<IGamePlayer> Opponents
        {
            get
            {
                var players = _playersRegistry
                    .SortedByOrderPlayers
                    .Where(p => !p.IsOwner)
                    .ToList();

                return players;
            }
        }

        public int NumberOfPlanetsOnPlayer => 
            NumberPlanetsOnPlayer;
    }
}