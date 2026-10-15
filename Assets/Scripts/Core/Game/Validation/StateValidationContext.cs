using System.Collections.Generic;
using System;

namespace Core.Game.Validation
{
    public sealed class StateValidationContext
    {
        private readonly StateValidationSession _session;

        internal StateValidationContext(StateValidationSession session, string path, ulong? ownerPlayerId)
        {
            _session = session;
            Path = path;
            OwnerPlayerId = ownerPlayerId;
        }

        public string Path { get; }

        public ulong? OwnerPlayerId { get; }

        public GameContextRole Role => _session.Role;

        public ulong LocalPlayerId => _session.LocalPlayerId;

        public long Revision => _session.Revision;

        public StateValidationContext At(string suffix)
        {
            return new StateValidationContext(_session, Path + suffix, OwnerPlayerId);
        }

        public void Require(bool condition, string message)
        {
            if (!condition)
            {
                throw new StateValidationException(Path, message);
            }
        }

        public void RegisterIdentity(Type entityType, object id)
        {
            Require(_session.GetIdentities(entityType).Add(id), "An entity ID is duplicated.");
        }

        public void RequireReference(Type entityType, object id)
        {
            _session.Defer(() => Require(_session.GetIdentities(entityType).Contains(id), "The referenced entity does not exist."));
        }

        public void RequireAllIdentities(Type entityType, HashSet<object> ids)
        {
            _session.Defer(() => Require(_session.GetIdentities(entityType).SetEquals(ids),
                "The collection must contain exactly the declared entities."));
        }

        internal StateValidationContext WithOwner(ulong ownerPlayerId)
        {
            return new StateValidationContext(_session, Path, ownerPlayerId);
        }

        internal void Defer(Action check)
        {
            _session.Defer(check);
        }
    }
}
