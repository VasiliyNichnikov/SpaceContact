using Core.Game.Dto.Game.States;

namespace Core.Game.Planets
{
    public class SpaceShip : ISpaceShip
    {
        public SpaceShip(int id, ulong ownerId)
        {
            Id = id;
            OwnerId = ownerId;
        }
        
        public int Id { get; }

        public ulong OwnerId { get; }

        public SpaceShipStateData ToData()
        {
            return new SpaceShipStateData
            {
                ShipId = Id,
                OwnerId = OwnerId,
            };
        }
    }
}