using System;

namespace Core.Game.Visibility
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class StateVisibilityAttribute : Attribute
    {
        public StateVisibilityAttribute(StateAudience audience)
        {
            Audience = audience;
        }

        public StateAudience Audience { get; }

        public bool IsOwnerId { get; set; }

        public bool Unordered { get; set; }
    }
}
