using Core.Game.Dto.Snapshots;

namespace Core.Game
{
    public sealed class SpaceShipData
    {
        public SpaceShipData(int shipId)
        {
            Id = shipId;
        }
        
        public int Id { get; }
        
        public static SpaceShipData Convert(SpaceShipSnapshot snapshot)
        {
            return new SpaceShipData(snapshot.ShipId);
        }
    }
}