using System.Linq;
using Core.Game.Cards;
using Core.Game.Players;
using Core.User;

namespace Core.Game
{
    public sealed class GamePlayersLoader
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
            var sortedUsersBySeatNumber = _usersRepository.Users.OrderBy(user => user.SeatNumber);

            var order = 0;
            
            foreach (var user in sortedUsersBySeatNumber)
            {
                var gamePlayer = CreateGamePlayer(user, order++);
                _gamePlayersRegistry.AddPlayer(gamePlayer, user.IsCurrentPlayer);
            }
        }

        private IGamePlayer CreateGamePlayer(IUser user, int order)
        {
            IGamePlayer player;
            var playerColor = _usersColorProvider.GetColor(user.ColorId);
            
            if (_networkContext.IsServer)
            {
                player = new ServerGamePlayer(user, _spaceCardFactory, playerColor, order);
            }
            else if (user.IsCurrentPlayer)
            {
                player = new SelfGamePlayer(user, _spaceCardFactory, playerColor, order);
            }
            else
            {
                player = new PublicGamePlayer(user, playerColor, order);
            }
            
            return player;
        }
    }
}