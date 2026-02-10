using System;
using Core.Game.Dto.Game.States;

namespace Core.Game
{
    public interface IGalaxyManagerNetwork
    {
        event Action? OnStateChanged;
        
        void ApplyStateData(GalaxyStateData data);
        
        GalaxyStateData GetState();
    }
}