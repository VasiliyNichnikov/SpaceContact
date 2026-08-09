using System.Collections.Generic;
using Core.Game.Events;

namespace Core.Game.Actions
{
    public sealed class GameActionResult
    {
        private GameActionResult(
            bool isSuccess,
            string? errorMessage,
            IReadOnlyList<IGameEvent> events)
        {
            IsSuccess = isSuccess;
            ErrorMessage = errorMessage;
            Events = events;
        }
        
        public bool IsSuccess { get; }
        
        public string? ErrorMessage { get; }

        public IReadOnlyList<IGameEvent> Events { get; }

        public static GameActionResult Failed(string errorMessage, IReadOnlyList<IGameEvent>? events = null)
        {
            events ??= new List<IGameEvent>();
            
            return new GameActionResult(false, errorMessage, events);
        }

        public static GameActionResult Success(IReadOnlyList<IGameEvent> events)
        {
            return new GameActionResult(true, null, events);
        }
        
        public static GameActionResult Success(IGameEvent evt)
        {
            var events = new List<IGameEvent>
            {
                evt
            };
            
            return new GameActionResult(true, null, events);
        }
    }
}