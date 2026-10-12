using System;
using System.Collections.Generic;
using System.Linq;
using Core.Game.Dto.Snapshots;

namespace Core.Game
{
    public sealed class GalaxyContext : IContext<GalaxySnapshot>
    {
        private Dictionary<ulong, List<PlanetData>> _planetsByPlayerId = new();
        private Dictionary<ulong, List<SpaceShipData>> _shipsByPlayerId = new();

        public IReadOnlyCollection<PlanetData> Planets
        {
            get
            {
                var planets = new List<PlanetData>();

                foreach (var pair in _planetsByPlayerId)
                {
                    planets.AddRange(pair.Value);
                }

                return planets.AsReadOnly();
            }
        }

        public ulong GetPlanetOwnerId(int planetId)
        {
            foreach (var (playerId, planets) in _planetsByPlayerId)
            {
                if (planets.Any(p => p.Id == planetId))
                {
                    return playerId;
                }
            }

            throw new ArgumentOutOfRangeException(nameof(planetId));
        }

        public IReadOnlyList<PlanetData> GetPlanetsByOwner(ulong playerId)
        {
            return _planetsByPlayerId[playerId];
        }

        public bool IsPlanetOwnedBy(ulong playerId, int planetId)
        {
            if (_planetsByPlayerId.TryGetValue(playerId, out var planets))
            {
                return planets.Any(p => p.Id == planetId);
            }

            return false;
        }

        public void LoadSnapshot(GalaxySnapshot snapshot)
        {
            var shipsByPlayerId = new Dictionary<ulong, List<SpaceShipData>>();
            var planetsByPlayerId = new Dictionary<ulong, List<PlanetData>>();

            foreach (var (playerId, ships) in snapshot.Ships)
            {
                shipsByPlayerId.Add(playerId, ships.Select(SpaceShipData.Convert).ToList());
            }

            foreach (var (playerId, planets) in snapshot.Planets)
            {
                planetsByPlayerId.Add(playerId, planets.Select(PlanetData.Convert).ToList());
            }

            _shipsByPlayerId = shipsByPlayerId;
            _planetsByPlayerId = planetsByPlayerId;
        }

        public GalaxySnapshot ToSnapshot()
        {
            var snapshot = new GalaxySnapshot();

            foreach (var (playerId, ships) in _shipsByPlayerId)
            {
                snapshot.Ships.Add(
                    playerId,
                    ships.Select(ship => new SpaceShipSnapshot
                    {
                        ShipId = ship.Id
                    }).ToList());
            }

            foreach (var (playerId, planets) in _planetsByPlayerId)
            {
                snapshot.Planets.Add(
                    playerId,
                    planets.Select(planet => new PlanetSnapshot
                    {
                        PlanetId = planet.Id,
                        ShipIds = planet.ShipIds.ToList()
                    }).ToList());
            }

            return snapshot;
        }

        internal void ReplaceState(GalaxyContext prepared)
        {
            _shipsByPlayerId = prepared._shipsByPlayerId;
            _planetsByPlayerId = prepared._planetsByPlayerId;
        }
    }
}
