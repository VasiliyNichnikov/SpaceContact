using System;
using Core.Game.Dto.States;

namespace Core.Game.Encounter
{
    public interface IGameServerEncounterManager
    {
        /// <summary>
        /// Срабатывает при начале нового столкновения
        /// </summary>
        event Action? Started;
        
        ulong? AggressorPlayerId { get; }
        
        ulong? DefenderPlayerId { get; }
        
        void StartEncounter();
        
        void SetDefenderPlayerId(ulong playerId);
        
        EncounterStateData ToState();
    }
}