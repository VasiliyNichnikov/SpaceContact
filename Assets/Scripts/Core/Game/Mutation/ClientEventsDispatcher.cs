using System.Collections.Generic;
using Logs;

namespace Core.Game.Mutation
{
    public sealed class ClientEventsDispatcher
    {
        private readonly List<int> _appliedEventIds = new();
        private int _lastEventId = int.MinValue;

        public void ApplyEvents(IEnumerable<IClientGameEvent> gameEvents)
        {
            foreach (var gameEvent in gameEvents)
            {
                if (_appliedEventIds.Contains(gameEvent.EventId))
                {
                    Logger.Error($"{nameof(ClientEventsDispatcher)}.{nameof(ApplyEvents)}: event with id {gameEvent.EventId} is already in use.");
                    continue;
                }

                if (_lastEventId > gameEvent.EventId)
                {
                    Logger.Error($"{nameof(ClientEventsDispatcher)}.{nameof(ApplyEvents)}: event with id {gameEvent.EventId} the event id is larger than the last event.");
                    continue;
                }
                
                _appliedEventIds.Add(gameEvent.EventId);
                _lastEventId = gameEvent.EventId;
                gameEvent.Apply();
                Logger.Log($"{nameof(ClientEventsDispatcher)}.{nameof(ApplyEvents)}: event with id {gameEvent.EventId} ({gameEvent.GetType()}) applied.");
            }
        }
    }
}