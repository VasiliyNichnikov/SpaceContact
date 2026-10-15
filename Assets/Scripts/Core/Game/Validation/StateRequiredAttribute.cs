namespace Core.Game.Validation
{
    public sealed class StateRequiredAttribute : StateValidationAttribute
    {
        public override void Validate(object? value, StateValidationContext context)
        {
            context.Require(value is not null, "A value is required for this audience.");
        }
    }
}
