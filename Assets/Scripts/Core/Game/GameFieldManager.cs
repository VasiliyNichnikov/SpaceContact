using System.Linq;
using Core.Game.Players;
using Logs;

namespace Core.Game
{
    public class GameFieldManager : IGameFieldManager
    {
        private readonly GamePlayersRegistry _playersRegistry;
        
        private IGamePlayer? _loadedCurrentPlayer;
        private IGamePlayer? _loadedOpponentPlayer;
        
        public GameFieldManager(GamePlayersRegistry playersRegistry)
        {
            _playersRegistry = playersRegistry;
        }

        public IGamePlayer CurrentPlayer
        {
            get
            {
                if (_loadedCurrentPlayer == null)
                {
                    Logger.Error("GameFieldManager.CurrentPlayer is null.");
                    
                    return EmptyGamePlayer.Instance;
                }
                
                return _loadedCurrentPlayer;
            }
        }

        public IGamePlayer OpponentPlayer
        {
            get
            {
                if (_loadedOpponentPlayer == null)
                {
                    Logger.Error("GameFieldManager.OpponentPlayer is null.");
                    
                    return EmptyGamePlayer.Instance;
                }
                
                return _loadedOpponentPlayer;
            }
        }

        public void Init()
        {
            var players = _playersRegistry.Players;
            
            _loadedCurrentPlayer = players.First(p => p.IsOwner);
            _loadedOpponentPlayer = players.First(p => !p.IsOwner);
        }
    }
}