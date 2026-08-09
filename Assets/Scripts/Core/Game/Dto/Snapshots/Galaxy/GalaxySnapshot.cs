using System;
using System.Collections.Generic;

namespace Core.Game.Dto.Snapshots
{
    [Serializable]
    public sealed class GalaxySnapshot
    {
        /// <summary>
        /// Key - player id,
        /// Value - owned planets
        /// </summary>
        public Dictionary<ulong, List<PlanetSnapshot>> Planets = new();
        
        /// <summary>
        /// Key - player id,
        /// Value - owned ships
        /// </summary>
        public Dictionary<ulong, List<SpaceShipSnapshot>> Ships = new();
    }
}