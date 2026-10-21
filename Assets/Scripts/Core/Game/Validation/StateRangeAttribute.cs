using System.Globalization;
using System;

namespace Core.Game.Validation
{
    public sealed class StateRangeAttribute : StateValidationAttribute
    {
        public StateRangeAttribute(double minimum, double maximum)
        {
            Minimum = minimum;
            Maximum = maximum;
        }

        public double Minimum { get; }

        public double Maximum { get; }

        public override void Validate(object? value, StateValidationContext context)
        {
            if (value is null)
            {
                return;
            }

            var number = Convert.ToDouble(value, CultureInfo.InvariantCulture);
            context.Require(!double.IsNaN(number) && !double.IsInfinity(number) && number >= Minimum && number <= Maximum,
                "The value must be finite and within its declared range.");
        }
    }
}
