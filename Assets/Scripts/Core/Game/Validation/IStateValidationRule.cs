namespace Core.Game.Validation
{
    public interface IStateValidationRule
    {
        void Validate(object value, StateValidationContext context);
    }
}
