using System;
using System.Collections.Generic;
using Core.EngineData;

namespace Core.Lobby
{
    public interface ILobbyColorProvider
    {
        event Action? OnColorsRefreshed;
        
        IReadOnlyCollection<Color> AllAvailablePlayerColors { get; }
        
        bool IsColorAvailableForSelection(Color color);
    }
}