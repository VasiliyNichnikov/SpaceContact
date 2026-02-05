using System;
using Core.EngineData;

namespace Core.Player
{
    /// <summary>
    /// Информация об игроке
    /// </summary>
    public interface IPlayerManager
    {
        string Name { get; }
        
        Color Color { get; }
        
        bool IsCurrentPlayer { get; }
        
        event Action? OnPlayerInfoUpdated;
    }
}