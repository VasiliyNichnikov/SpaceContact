namespace Core.Game.Encounter
{
    public interface IGameClientEncounterEvents
    {
        void SetAggressorEvent(ulong aggressorPlayerId);
        
        void SetDefenderEvent(ulong defenderPlayerId);

        void SetPlanetIdToAttack(int planetId);
    }
}