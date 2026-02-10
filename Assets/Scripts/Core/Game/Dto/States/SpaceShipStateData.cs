using System;

namespace Core.Game.Dto.States
{
    [Serializable]
    public struct SpaceShipStateData
    {
        public int ShipId;
        
        public ulong OwnerId;
    }
}