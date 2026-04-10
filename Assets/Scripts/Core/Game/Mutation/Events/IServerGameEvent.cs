namespace Core.Game.Mutation
{
    public interface IServerGameEvent
    {
        int EventId { get; }

        TState ToState<TState>(IGameEventToStateMapper<TState> mapper);
    }
}