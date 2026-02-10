using System;
using System.Collections.Generic;

namespace Core.Game.Dto.Game.States
{
    [Serializable]
    public class PlanetStateData
    {
        public int PlanetId;

        public ulong OwnerId;

        public List<SpaceShipStateData> Ships =  new();
    }
}