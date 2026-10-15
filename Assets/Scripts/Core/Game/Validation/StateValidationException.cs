using System;

namespace Core.Game.Validation
{
    public sealed class StateValidationException : InvalidOperationException
    {
        public StateValidationException(string path, string message) : base($"{path}: {message}")
        {
            Path = path;
        }

        public string Path { get; }
    }
}
