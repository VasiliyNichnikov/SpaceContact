namespace Core.Game.Validation
{
    public abstract class StateValidationRule<T> : IStateValidationRule
    {
        public void Validate(object value, StateValidationContext context)
        {
            Validate((T)value, context);
        }

        protected abstract void Validate(T value, StateValidationContext context);
    }
}
