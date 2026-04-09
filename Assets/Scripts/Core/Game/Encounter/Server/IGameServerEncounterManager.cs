using Core.Game.Dto.States;

namespace Core.Game.Encounter
{
    public interface IGameServerEncounterManager
    {
        ulong? AggressorPlayerId { get; }
        
        ulong? DefenderPlayerId { get; }
        
        void StartEncounter();
        
        void SetDefenderPlayerId(ulong playerId);
        
        EncounterStateData ToState();
    }
}