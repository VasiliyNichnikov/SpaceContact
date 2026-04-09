using System;
using Core.Game.Dto.States;
using Core.Game.Players;

namespace Core.Game.Encounter
{
    public interface IGameClientEncounterManager
    {
        event Action? StateChanged;
        
        event Action? AggressorChanged;
        
        event Action? DefenderChanged;
        
        IGamePlayer? AggressorPlayer { get; }
        
        IGamePlayer? DefenderPlayer { get; }
        
        void UpdateState(EncounterStateData state);
    }
}