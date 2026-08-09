namespace Core.Game.Actions
{
    public struct GameActor
    {
        public ulong? PlayerId { get; }
        
        public bool IsSystem { get; }

        private GameActor(ulong? playerId, bool isSystem)
        {
            PlayerId = playerId;
            IsSystem = isSystem;
        }

        public static GameActor Player(ulong playerId) => 
            new(playerId, false);
        
        public static GameActor System() => 
            new (null, true);
    }
}