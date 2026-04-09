namespace Core.Game.Mutation
{
    public class GameClientErrorEvent : IClientGameEvent
    {
        private static GameClientErrorEvent? _instance;
        private const int InvalidEventId = int.MinValue;
        
        private GameClientErrorEvent()
        {
            // nothing
        }

        public static IClientGameEvent Instance => 
            _instance ??= new GameClientErrorEvent();
        
        public int EventId => 
            InvalidEventId;
        
        public void Apply()
        {
            // nothing
        }
    }
}