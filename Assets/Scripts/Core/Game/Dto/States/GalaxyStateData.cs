using System;
using System.Collections.Generic;

namespace Core.Game.Dto.States
{
    [Serializable]
    public class GalaxyStateData
    {
        public List<PlanetStateData> Planets = new();
    }
}