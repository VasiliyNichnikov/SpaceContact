using System;
using Core.Game.Dto.States;

namespace Core.Game
{
    public interface IGalaxyManagerNetwork
    {
        event Action? OnStateChanged;
        
        void ApplyStateData(GalaxyStateData data);
        
        GalaxyStateData GetState();
    }
}