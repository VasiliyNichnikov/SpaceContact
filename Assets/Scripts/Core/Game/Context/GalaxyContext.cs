using System;
using System.Collections.Generic;
using System.Linq;
using Core.Game.Dto.Snapshots;

namespace Core.Game
{
    public sealed class GalaxyContext : IContext<GalaxySnapshot>
    {
        private readonly Dictionary<ulong, List<PlanetData>> _planetsByPlayerId = new();
        private readonly Dictionary<ulong, List<SpaceShipData>> _shipsByPlayerId = new();

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
            _planetsByPlayerId.Clear();
            _shipsByPlayerId.Clear();

            foreach (var (playerId, ships) in snapshot.Ships)
            {
                _shipsByPlayerId.Add(playerId, ships.Select(SpaceShipData.Convert).ToList());
            }

            foreach (var (playerId, planets) in snapshot.Planets)
            {
                _planetsByPlayerId.Add(playerId, planets.Select(PlanetData.Convert).ToList());
            }
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
    }
}
