using Core.Game.Dto.States;

namespace Core.Game.Encounter
{
    public interface IGameServerEncounterManager
    {
        void StartEncounter();
        
        void SetDefenderPlayerId(ulong playerId);

        bool SetPlanetToAttack(ulong initiatedByPlayerId, int planetId);
        
        EncounterStateData ToState();
    }
}