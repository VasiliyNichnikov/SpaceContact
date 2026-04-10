using Core.Game.Dto.States.Cards;

namespace Core.Game.Mutation.Events
{
    public sealed class GameServerDestinyCardChangedEvent : IServerGameEvent
    {
        public GameServerDestinyCardChangedEvent(
            int eventId,
            DestinyCardData destinyCardData)
        {
            EventId = eventId;
            DestinyCardData = destinyCardData;
        }
        
        public int EventId { get; }
        
        public DestinyCardData DestinyCardData { get; }
        
        public TState ToState<TState>(IGameEventToStateMapper<TState> mapper) => 
            mapper.Visit(this);
    }
}