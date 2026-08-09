using System.Collections.Generic;
using Core.Game.Dto.Snapshots;

namespace Core.Game
{
    public sealed class PlanetData
    {
        private readonly List<int> _shipIds;
        
        public PlanetData(int planetId, List<int> shipIds)
        {
            Id = planetId;
            _shipIds = shipIds;
        }
        
        public int Id { get; }

        public IReadOnlyCollection<int> ShipIds => _shipIds;

        public static PlanetData Convert(PlanetSnapshot snapshot)
        {
            return new PlanetData(snapshot.PlanetId, snapshot.ShipIds);
        }
    }
}