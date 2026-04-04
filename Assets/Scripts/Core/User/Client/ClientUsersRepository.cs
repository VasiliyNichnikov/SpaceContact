using System;
using System.Collections.Generic;
using System.Linq;
using Core.User.Dto.States;

namespace Core.User
{
    public sealed class ClientUsersRepository
    {
        private readonly Dictionary<ulong, ClientUser> _users = new();
        private readonly ClientUserFactory _userFactory;

        public ClientUsersRepository(ClientUserFactory userFactory)
        {
            _userFactory = userFactory;
        }
        
        public event Action<IUser>? OnUserJoined;
        
        public event Action<IUser>? OnUserLeft;
        
        public IReadOnlyCollection<IUser> Users => 
            _users.Values;

        public IUser GetCurrentUser()
        {
            var kvp = _users.First(u => u.Value.IsCurrentPlayer);
            
            return kvp.Value;
        }
        
        public void Apply(ulong localClientId, UserStateData state)
        {
            if (_users.TryGetValue(state.UserId, out var user))
            {
                user.Apply(state);
                return;
            }

            var createdUser = _userFactory.Create(localClientId, state);
            _users[state.UserId] = createdUser;
            OnUserJoined?.Invoke(createdUser);
            createdUser.Init();
        }
        
        public void Remove(ulong userId)
        {
            if (!_users.Remove(userId, out var user))
            {
                return;
            }
            
            user.OnLeftGame();
            OnUserLeft?.Invoke(user);
        }

    }
}