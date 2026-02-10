using System;

namespace Core.Game.Dto.Game.States
{
    [Serializable]
    public struct SpaceShipStateData
    {
        public int ShipId;
        
        public ulong OwnerId;
    }
}