namespace Core.Game.Mutation.Events
{
    public class GameServerAggressorSelectedEvent : IServerGameEvent
    {
        public GameServerAggressorSelectedEvent(
            int eventId, 
            ulong aggressorPlayerId)
        {
            EventId = eventId;
            AggressorPlayerId = aggressorPlayerId;
        }
        
        public ulong AggressorPlayerId { get; }
        
        public int EventId { get; }

        public GameEventType EventType => 
            GameEventType.AggressorSelected;

        public TState ToState<TState>(IGameEventToStateMapper<TState> mapper) => 
            mapper.Visit(this);
    }
}