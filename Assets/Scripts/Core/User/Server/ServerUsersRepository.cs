using System.Collections.Generic;

namespace Core.User
{
    public class ServerUsersRepository
    {
        private readonly Dictionary<ulong, IServerUser> _users = new();
        
        public bool ContainsUser(ulong clientId) => 
            _users.ContainsKey(clientId);
        
        public IServerUser Get(ulong clientId) => 
            _users[clientId];

        public void Add(IServerUser user) => 
            _users.Add(user.ClientId, user);

        public void Remove(IServerUser user) =>
            _users.Remove(user.ClientId);
    }
}