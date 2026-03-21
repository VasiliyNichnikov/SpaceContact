using System;
using Core.Game.Phases;

namespace Core.Game
{
    public interface IGameStateMachineReadOnly
    {
        IGamePhase? CurrentPhase { get; }
        
        event Action? OnPhaseChanged;
    }
}