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
        
        bool IsOwnerLobby { get; }
        
        event Action? OnPlayerInfoUpdated;
        
        void SetName(string newName);
    }
}