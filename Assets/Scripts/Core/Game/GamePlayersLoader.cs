using Core.Game.Cards;
using Core.Game.Players;
using Core.User;

namespace Core.Game
{
    public class GamePlayersLoader
    {
        private readonly CoreNetworkContext _networkContext;
        private readonly ClientUsersRepository _usersRepository;
        private readonly GamePlayersRegistry _gamePlayersRegistry;
        private readonly IUsersColorProvider _usersColorProvider;

        private readonly SpaceCardFactory _spaceCardFactory;

        public GamePlayersLoader(
            ClientUsersRepository usersRepository,
            GamePlayersRegistry gamePlayersRegistry,
            CoreNetworkContext networkContext,
            IUsersColorProvider usersColorProvider,
            
            SpaceCardFactory spaceCardFactory)
        {
            _usersRepository = usersRepository;
            _gamePlayersRegistry = gamePlayersRegistry;
            _networkContext = networkContext;
            _spaceCardFactory = spaceCardFactory;
            _usersColorProvider = usersColorProvider;
        }

        public void Init() => 
            LoadPlayers();
        
        private void LoadPlayers()
        {
            foreach (var playerCore in _usersRepository.Users)
            {
                var gamePlayer = CreateGamePlayer(playerCore);
                _gamePlayersRegistry.AddPlayer(gamePlayer, playerCore.IsCurrentPlayer);
            }
        }

        private IGamePlayer CreateGamePlayer(IUser user)
        {
            IGamePlayer player;
            var playerColor = _usersColorProvider.GetColor(user.ColorId);
            
            if (_networkContext.IsServer)
            {
                player = new ServerGamePlayer(user, _spaceCardFactory, playerColor);
            }
            else if (user.IsCurrentPlayer)
            {
                player = new OwnerGamePlayer(user, _spaceCardFactory, playerColor);
            }
            else
            {
                player = new SimpleGamePlayer(user, playerColor);
            }
            
            return player;
        }
    }
}