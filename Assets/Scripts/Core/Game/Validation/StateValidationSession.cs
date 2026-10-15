using System.Collections.Generic;
using System;

namespace Core.Game.Validation
{
    internal sealed class StateValidationSession
    {
        private readonly Dictionary<Type, HashSet<object>> _identities = new();
        private readonly List<Action> _pending = new();

        public StateValidationSession(GameContextRole role, ulong localPlayerId, long revision)
        {
            Role = role;
            LocalPlayerId = localPlayerId;
            Revision = revision;
        }

        public GameContextRole Role { get; }

        public ulong LocalPlayerId { get; }

        public long Revision { get; }

        public HashSet<object> GetIdentities(Type entityType)
        {
            if (!_identities.TryGetValue(entityType, out var ids))
            {
                ids = new HashSet<object>();
                _identities.Add(entityType, ids);
            }

            return ids;
        }

        public void Defer(Action check)
        {
            _pending.Add(check);
        }

        public void Complete()
        {
            for (var i = 0; i < _pending.Count; i++)
            {
                _pending[i]();
            }
        }
    }
}
