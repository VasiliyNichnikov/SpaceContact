using System.Collections.Generic;
using System.Linq;
using Core.Game.Dto.Rules;
using Core.Game.Dto.Snapshots;

namespace Core.Game.Galaxy
{
    public sealed class GalaxyInitialStateFactory
    {
        private readonly RulesOfPlanetsData _rules;

        public GalaxyInitialStateFactory(RulesOfPlanetsData rules)
        {
            _rules = rules;
        }

        public GalaxySnapshot Create(IReadOnlyCollection<GamePlayerSnapshot> players)
        {
            var nextShipId = 0;
            var nextPlanetId = 0;
            var snapshot = new GalaxySnapshot();

            var sortedPlayers =
                players
                .OrderBy(p => p.Order);

            foreach (var player in sortedPlayers)
            {
                var ships = new List<SpaceShipSnapshot>();
                var planets = CreatePlanetsForPlayer(ref nextPlanetId, ref nextShipId, ref ships);

                snapshot.Planets.Add(player.PlayerId, planets);
                snapshot.Ships.Add(player.PlayerId, ships);
            }

            return snapshot;
        }

        private List<PlanetSnapshot> CreatePlanetsForPlayer(
            ref int nextPlanetId,
            ref int nextShipId,
            ref List<SpaceShipSnapshot> ships)
        {
            var planets = new List<PlanetSnapshot>();

            for (var i = 0; i < _rules.NumberOfPlanetsPlayer; i++)
            {
                var planet = new PlanetSnapshot
                {
                    PlanetId = nextPlanetId++,
                    ShipIds = CreateShipsForPlanet(ref nextShipId, ref ships)
                };

                planets.Add(planet);
            }

            return planets;
        }

        private List<int> CreateShipsForPlanet(ref int nextShipId, ref List<SpaceShipSnapshot> ships)
        {
            var shipIds = new List<int>();

            for (var i = 0; i < _rules.NumberOfShipsOnPlanet; i++)
            {
                var shipId = nextShipId++;
                var ship = new SpaceShipSnapshot
                {
                    ShipId = shipId
                };

                ships.Add(ship);
                shipIds.Add(shipId);
            }

            return shipIds;
        }
    }
}
