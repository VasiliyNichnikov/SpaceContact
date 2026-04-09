using System;
using System.Collections.Generic;
using System.Linq;
using Core.Game.Dto.States;
using Core.Game.Planets;
using Core.Game.Players;
using Core.Game.Players.Visitors;

namespace Core.Game.Galaxy
{
    public sealed class GameClientGalaxyManager : IGameClientGalaxyManager
    {
        private readonly GamePlayersRegistry _playersRegistry;
        
        private readonly Dictionary<int, Planet> _planetById = new();
        
        public GameClientGalaxyManager(GamePlayersRegistry playersRegistry)
        {
            _playersRegistry = playersRegistry;
        }

        public event Action? StateChanged;

        public void UpdateState(GalaxyStateData state)
        {
            RefreshState(state);
            UpdatePlayersPlanets();
            StateChanged?.Invoke();
        }

        private void RefreshState(GalaxyStateData data)
        {
            _planetById.Clear();

            foreach (var planetData in data.Planets)
            {
                var ships = planetData
                    .Ships
                    .Select(ship => new SpaceShip(ship.ShipId, ship.OwnerId));
                var createdPlanet = new Planet(planetData.PlanetId, planetData.OwnerId, ships);
                _planetById.Add(planetData.PlanetId, createdPlanet);
            }
            
            StateChanged?.Invoke();
        }
        
        private void UpdatePlayersPlanets()
        {
            foreach (var player in _playersRegistry.Players)
            {
                var planetsUploaderVisitor = new GamePlayerPlanetsUpdaterVisitor(GetPlayerPlanets);
                player.Apply(planetsUploaderVisitor);
            }
        }
        
        private IReadOnlyCollection<IPlanet> GetPlayerPlanets(ulong playerId)
        {
            var planets = _planetById.Values
                .Where(planet => planet.OwnerId == playerId)
                .ToList();

            return planets;
        }
    }
}