using System.Collections.Generic;
using Core.Game.Planets;

namespace Core.Game
{
    public interface IGalaxyManager
    {
        void Init();
        
        IReadOnlyCollection<IPlanet> GetPlayerPlanets(ulong playerId);
    }
}