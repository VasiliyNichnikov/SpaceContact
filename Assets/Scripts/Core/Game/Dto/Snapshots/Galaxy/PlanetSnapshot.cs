using System;
using System.Collections.Generic;

namespace Core.Game.Dto.Snapshots
{
    [Serializable]
    public struct PlanetSnapshot
    {
        public int PlanetId;

        public List<int> ShipIds;
    }
}