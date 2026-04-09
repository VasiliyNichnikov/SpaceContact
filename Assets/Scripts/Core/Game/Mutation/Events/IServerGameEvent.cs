namespace Core.Game.Mutation
{
    public interface IServerGameEvent
    {
        int EventId { get; }
        
        GameEventType EventType { get; }

        TState ToState<TState>(IGameEventToStateMapper<TState> mapper);
    }
}