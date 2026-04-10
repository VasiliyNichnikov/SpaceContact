namespace Core.Game.Mutation.Events
{
    public sealed class GameServerDefenderSelectedEvent : IServerGameEvent
    {
        public GameServerDefenderSelectedEvent(
            int eventId,
            ulong defenderPlayerId)
        {
            EventId = eventId;
            DefenderPlayerId = defenderPlayerId;
        }
        
        public ulong DefenderPlayerId { get; }
        
        public int EventId { get; }

        public TState ToState<TState>(IGameEventToStateMapper<TState> mapper) => 
            mapper.Visit(this);
    }
}