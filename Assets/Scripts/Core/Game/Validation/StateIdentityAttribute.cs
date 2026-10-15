using System;

namespace Core.Game.Validation
{
    public sealed class StateIdentityAttribute : StateValidationAttribute
    {
        public StateIdentityAttribute(Type entityType)
        {
            EntityType = entityType;
        }

        public Type EntityType { get; }

        public override void Validate(object? value, StateValidationContext context)
        {
            if (value is not null)
            {
                context.RegisterIdentity(EntityType, value);
            }
        }
    }
}
