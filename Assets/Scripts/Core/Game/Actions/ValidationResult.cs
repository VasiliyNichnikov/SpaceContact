using System;

namespace Core.Game.Actions
{
    public readonly struct ValidationResult
    {
        public const int SuccessfulResult = int.MinValue;

        public readonly int Result;

        public readonly string? ErrorMessage;

        private ValidationResult(int result, string errorMessage)
        {
            Result = result;
            ErrorMessage = errorMessage;
        }

        public static ValidationResult Ok => new(SuccessfulResult, string.Empty);

        public bool IsSuccess => Result == SuccessfulResult;

        public static ValidationResult Failed(int result, string errorMessage)
        {
            if (result == SuccessfulResult)
            {
                throw new ArgumentOutOfRangeException(nameof(result), "The success code cannot identify a failure.");
            }

            return new ValidationResult(result, errorMessage);
        }
    }
}
