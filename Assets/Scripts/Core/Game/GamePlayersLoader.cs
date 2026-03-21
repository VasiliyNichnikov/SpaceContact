using Core.Game.Cards;
using Core.Game.Players;
using Core.Player;

namespace Core.Game
{
    public class GamePlayersLoader
    {
        private readonly CoreNetworkContext _networkContext;
        private readonly PlayersRegistry _playersRegistry;
        private readonly GamePlayersRegistry _gamePlayersRegistry;

        private readonly SpaceCardFactory _spaceCardFactory;

        public GamePlayersLoader(
            PlayersRegistry playersRegistry,
            GamePlayersRegistry gamePlayersRegistry,
            CoreNetworkContext networkContext,
            
            SpaceCardFactory spaceCardFactory)
        {
            _playersRegistry = playersRegistry;
            _gamePlayersRegistry = gamePlayersRegistry;
            _networkContext = networkContext;
            _spaceCardFactory = spaceCardFactory;
        }

        public void Init() => 
            LoadPlayers();
        
        private void LoadPlayers()
        {
            foreach (var playerCore in _playersRegistry.Players)
            {
                var gamePlayer = CreateGamePlayer(playerCore);
                _gamePlayersRegistry.AddPlayer(gamePlayer, playerCore.IsCurrentPlayer);
            }
        }

        private IGamePlayer CreateGamePlayer(IPlayerManager playerCore)
        {
            IGamePlayer player;
                
            if (_networkContext.IsServer)
            {
                player = new ServerGamePlayer(playerCore, _spaceCardFactory);
            }
            else if (playerCore.IsCurrentPlayer)
            {
                player = new OwnerGamePlayer(playerCore, _spaceCardFactory);
            }
            else
            {
                player = new SimpleGamePlayer(playerCore);
            }
            
            return player;
        }
    }
}