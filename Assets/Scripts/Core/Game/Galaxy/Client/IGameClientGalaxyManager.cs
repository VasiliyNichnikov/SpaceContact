using System;
using Core.Game.Dto.States;

namespace Core.Game.Galaxy
{
    public interface IGameClientGalaxyManager
    {
        event Action? StateChanged;
        
        void UpdateState(GalaxyStateData state);
    }
}