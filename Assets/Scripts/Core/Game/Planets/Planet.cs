using System.Collections.Generic;

namespace Core.Game.Planets
{
    public class Planet : IPlanet
    {
        private readonly int _id;
        private readonly ulong _ownerId;
        private readonly List<SpaceShip> _ships = new();
        
        public Planet(int id, ulong ownerId, IEnumerable<SpaceShip> initialShips)
        {
            _id = id;
            _ownerId = ownerId;
            _ships.AddRange(initialShips);
        }
        
        public int Id => _id;
        
        public ulong OwnerId => _ownerId;

        public IReadOnlyCollection<ISpaceShip> Ships => _ships;
    }
}