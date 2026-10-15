using System.Collections;

namespace Core.Game.Validation
{
    public sealed class StateNotEmptyAttribute : StateValidationAttribute
    {
        public override void Validate(object? value, StateValidationContext context)
        {
            if (value is null)
            {
                return;
            }

            context.Require(value is string text ? !string.IsNullOrWhiteSpace(text) : value is ICollection { Count: > 0 },
                "The value must not be empty.");
        }
    }
}
