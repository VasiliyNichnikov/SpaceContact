namespace Core.Game.Mutation.Events
{
    public sealed class GameServerPlanetToAttackSelectedEvent : IServerGameEvent
    {
        public GameServerPlanetToAttackSelectedEvent(int eventId, ulong initiatedByPlayerId, int planetId)
        {
            EventId = eventId;
            InitiatedByPlayerId = initiatedByPlayerId;
            PlanetId = planetId;
        }
        
        public int EventId { get; }
        
        public ulong InitiatedByPlayerId { get; }
        
        public int PlanetId { get; }

        public TState ToState<TState>(IGameEventToStateMapper<TState> mapper) => 
            mapper.Visit(this);
    }
}