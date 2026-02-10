using System.Linq;
using Core.Player;
using Logs;

namespace Core.Game
{
    public class TwoPlayerFieldManager : ITwoPlayerFieldManager
    {
        private readonly PlayersRegistry _playersRegistry;
        private readonly IGalaxyManager _galaxyManager;
        
        public TwoPlayerFieldManager(
            PlayersRegistry playersRegistry, 
            IGalaxyManager galaxyManager)
        {
            _playersRegistry = playersRegistry;
            _galaxyManager = galaxyManager;
        }

        public GamePlayer CurrentPlayer { get; private set; } = null!;
        
        public GamePlayer OpponentPlayer { get; private set; } = null!;
        
        public void Init()
        {
            CurrentPlayer = CreateCurrentGamePlayer();
            OpponentPlayer = CreateOpponentGamePlayer();
        }
        
        private GamePlayer CreateCurrentGamePlayer()
        {
            var currentPlayer = _playersRegistry.CurrentPlayer;
            
            if (currentPlayer == null)
            {
                Logger.Error("TwoPlayerFieldManager.CurrentPlayer is null");
                
                return null!;
            }
            
            var planets = _galaxyManager.GetPlayerPlanets(currentPlayer.ClientId);

            return new GamePlayer(currentPlayer, planets);
        }

        private GamePlayer CreateOpponentGamePlayer()
        {
            var firstOtherPlayer = _playersRegistry.OtherPlayers.First();
            var planets = _galaxyManager.GetPlayerPlanets(firstOtherPlayer.ClientId);
            
            return new GamePlayer(firstOtherPlayer, planets);
        }
    }
}