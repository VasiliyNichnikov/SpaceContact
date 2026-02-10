using System;
using System.Collections.Generic;
using System.Linq;
using Core.Game.Dto.States;

namespace Core.Game.Planets
{
    public class Planet : IPlanet
    {
        private readonly int _id;
        private readonly ulong _ownerId;
        private readonly List<SpaceShip> _ships = new();
        
        public Planet(int id, ulong ownerId, IReadOnlyCollection<SpaceShip> initialShips)
        {
            _id = id;
            _ownerId = ownerId;
            _ships.AddRange(initialShips);
        }

        public event Action? OnShipsChanged;
        
        public int Id => _id;
        
        public ulong OwnerId => _ownerId;

        public IReadOnlyCollection<ISpaceShip> Ships => _ships;

        public void UpdateState(PlanetStateData stateData)
        {
            _ships.Clear();
            var ships = stateData.Ships.Select(CreateSpaceShip);
            _ships.AddRange(ships);
            
            OnShipsChanged?.Invoke();
        }

        public PlanetStateData ToData()
        {
            var ships = _ships
                .Select(s => s.ToData())
                .ToList();

            return new PlanetStateData
            {
                PlanetId = _id,
                OwnerId = _ownerId,
                Ships = ships
            };
        }
        
        private static SpaceShip CreateSpaceShip(SpaceShipStateData stateData)
        {
            return new SpaceShip(stateData.ShipId, stateData.OwnerId);
        }
    }
}