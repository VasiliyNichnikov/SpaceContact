using System;

namespace Core.Game.Actions
{
    internal readonly struct GameActionKey : IEquatable<GameActionKey>
    {
        public GameActionKey(GameActor actor, Guid actionId)
        {
            IsSystem = actor.IsSystem;
            PlayerId = actor.PlayerId;
            ActionId = actionId;
        }

        public bool IsSystem { get; }

        public ulong? PlayerId { get; }

        public Guid ActionId { get; }

        public bool Equals(GameActionKey other)
        {
            return IsSystem == other.IsSystem && PlayerId == other.PlayerId && ActionId == other.ActionId;
        }

        public override bool Equals(object? obj)
        {
            return obj is GameActionKey other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(IsSystem, PlayerId, ActionId);
        }
    }
}
