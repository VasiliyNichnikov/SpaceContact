using Core.Game.Players;
using Core.Player;

namespace Core.Game
{
    public class GamePlayersLoader
    {
        private readonly CoreNetworkContext _networkContext;
        private readonly PlayersRegistry _playersRegistry;
        private readonly GamePlayersRegistry _gamePlayersRegistry;

        public GamePlayersLoader(
            PlayersRegistry playersRegistry,
            GamePlayersRegistry gamePlayersRegistry,
            CoreNetworkContext networkContext)
        {
            _playersRegistry = playersRegistry;
            _gamePlayersRegistry = gamePlayersRegistry;
            _networkContext = networkContext;
        }

        public void Init() => 
            LoadPlayers();
        
        private void LoadPlayers()
        {
            foreach (var playerCore in _playersRegistry.Players)
            {
                var gamePlayer = CreateGamePlayer(playerCore);
                _gamePlayersRegistry.AddPlayer(gamePlayer);
            }
        }

        private IGamePlayer CreateGamePlayer(IPlayerManager playerCore)
        {
            IGamePlayer player;
                
            if (_networkContext.IsServer)
            {
                player = new ServerGamePlayer(playerCore);
            }
            else if (playerCore.IsCurrentPlayer)
            {
                player = new OwnerGamePlayer(playerCore);
            }
            else
            {
                player = new SimpleGamePlayer(playerCore);
            }
            
            return player;
        }
    }
}