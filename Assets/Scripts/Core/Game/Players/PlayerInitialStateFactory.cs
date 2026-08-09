using System.Linq;
using Core.Game.Dto.Snapshots;
using Core.User;

namespace Core.Game.Players
{
    public sealed class PlayerInitialStateFactory
    {
        private readonly ClientUsersRepository _users;
        private readonly IUsersColorProvider _colorProvider;

        public PlayerInitialStateFactory(
            ClientUsersRepository users, 
            IUsersColorProvider colorProvider)
        {
            _users = users;
            _colorProvider = colorProvider;
        }
        
        public GamePlayersSnapshot Create()
        {
            var sortedUsersBySeatNumber = _users
                .Users
                .OrderBy(user => user.SeatNumber);
            
            var order = 0;
            var state = new GamePlayersSnapshot();

            var players= sortedUsersBySeatNumber
                .Select(user => CreatePlayerState(user, order++))
                .ToList();
            state.Players.AddRange(players);
            
            return state;
        }

        private GamePlayerSnapshot CreatePlayerState(IUser user, int order)
        {
            var state = new GamePlayerSnapshot
            {
                PlayerId = user.ClientId,
                Name = user.Name,
                Order = order,
                Color = _colorProvider.GetColor(user.ColorId),
                IsReady = false
            };
            
            return state;
        }
    }
}