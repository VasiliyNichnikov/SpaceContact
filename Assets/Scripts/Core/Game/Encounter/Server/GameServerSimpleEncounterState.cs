namespace Core.Game.Encounter
{
    public class GameServerSimpleEncounterState
    {
        public ulong? AggressorPlayerId { get; private set; }
        
        public ulong? DefenderPlayerId { get; private set; }
        
        public void SetAggressorPlayerId(ulong? playerId) => 
            AggressorPlayerId = playerId;
        
        public void SetDefenderPlayerId(ulong? playerId) => 
            DefenderPlayerId = playerId;
    }
}