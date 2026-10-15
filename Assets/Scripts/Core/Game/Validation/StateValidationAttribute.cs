using System;

namespace Core.Game.Validation
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
    public abstract class StateValidationAttribute : Attribute
    {
        public abstract void Validate(object? value, StateValidationContext context);
    }
}
