namespace Core.Game.Mutation
{
    public interface IClientGameEvent
    {
        int EventId { get; }
        
        void Apply();
    }
}