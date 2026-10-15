using System.Collections.Generic;
using System;
using Core.Game.Visibility;
using Newtonsoft.Json.Serialization;

namespace Core.Game.Validation
{
    internal sealed class StateValidationMember
    {
        public StateValidationMember
        (string name,
            Type valueType,
            IValueProvider valueProvider,
            StateVisibilityAttribute visibility,
            IReadOnlyList<StateValidationAttribute> checks)
        {
            Name = name;
            ValueType = valueType;
            ValueProvider = valueProvider;
            Visibility = visibility;
            Checks = checks;
        }

        public string Name { get; }

        public Type ValueType { get; }

        public IValueProvider ValueProvider { get; }

        public StateVisibilityAttribute Visibility { get; }

        public IReadOnlyList<StateValidationAttribute> Checks { get; }
    }
}
