using System;

namespace Core.Game.Visibility
{
    internal sealed class StateProjectionMember
    {
        public StateProjectionMember(string name, Type valueType, StateVisibilityAttribute visibility)
        {
            Name = name;
            ValueType = valueType;
            Visibility = visibility;
        }

        public string Name { get; }

        public Type ValueType { get; }

        public StateVisibilityAttribute Visibility { get; }
    }
}
