using System;

namespace Core.Game.Validation
{
    public sealed class StateDefinedEnumAttribute : StateValidationAttribute
    {
        public override void Validate(object? value, StateValidationContext context)
        {
            if (value is not null)
            {
                context.Require(value.GetType().IsEnum && Enum.IsDefined(value.GetType(), value),
                    "The enum value is not defined.");
            }
        }
    }
}
