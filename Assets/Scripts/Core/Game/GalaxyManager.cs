using System;
using System.Collections.Generic;
using System.Linq;
using Core.Game.Dto.Game.Rules;
using Core.Game.Dto.Game.States;
using Core.Game.Planets;
using Core.Player;

namespace Core.Game
{
    public class GalaxyManager : IGalaxyManager, IGalaxyManagerNetwork
    {
        private readonly PlayersRegistry _playersRegistry;
        private readonly RulesOfPlanetsData _rulesOfPlanets;
        
        // key - planetId, value - planet 
        private readonly Dictionary<int, Planet> _planetById = new();
        
        public GalaxyManager(PlayersRegistry playersRegistry, RulesOfPlanetsData rulesOfPlanets)
        {
            _playersRegistry = playersRegistry;
            _rulesOfPlanets = rulesOfPlanets;
        }
        
        public event Action? OnStateChanged;
        
        void IGalaxyManager.Init()
        {
            InitPlanetsAndShips(_rulesOfPlanets);
        }

        IReadOnlyCollection<IPlanet> IGalaxyManager.GetPlayerPlanets(ulong playerId)
        {
            var planets = _planetById.Values
                .Where(planet => planet.OwnerId == playerId)
                .ToList();

            return planets;
        }

        void IGalaxyManagerNetwork.ApplyStateData(GalaxyStateData data)
        {
            RefreshGalaxyData(data);
        }

        GalaxyStateData IGalaxyManagerNetwork.GetState()
        {
            var state = new GalaxyStateData();
            var planets = _planetById
                .Values
                .Select(p => p.ToData());
            state.Planets.AddRange(planets);
            
            return state;
        }

        private void InitPlanetsAndShips(RulesOfPlanetsData rules)
        {
            _planetById.Clear();
            
            var shipId = 0;
            var planetId = 0;

            var sortedPlayers = _playersRegistry
                .Players
                .OrderBy(p => p.ClientId);
            
            var shipsByPlayer = new Dictionary<ulong, List<SpaceShip>>();
            
            foreach (var player in sortedPlayers)
            {
                for (var i = 0; i < rules.NumberOfShipsOnPlanet; i++)
                {
                    var createdShip = new SpaceShip(shipId, player.ClientId);
                    
                    if (shipsByPlayer.TryGetValue(player.ClientId, out var ships))
                    {
                        ships.Add(createdShip);
                    }
                    else
                    {
                        shipsByPlayer.Add(player.ClientId, new List<SpaceShip> { createdShip });
                    }
                    
                    shipId++;
                }
                
                for (var i = 0; i < rules.NumberOfPlanetsPlayer; i++)
                {
                    var shipsOnPlanet = shipsByPlayer[player.ClientId];
                    var createdPlanet = CreatePlanet(planetId, player.ClientId, shipsOnPlanet);
                    _planetById[planetId] = createdPlanet;
                    planetId++;
                }
            }
            
            OnStateChanged?.Invoke();
        }
        
        private void RefreshGalaxyData(GalaxyStateData data)
        {
            foreach (var planetData in data.Planets)
            {
                var planet = _planetById[planetData.PlanetId];
                planet.UpdateState(planetData);
            }
        }
        
        private static Planet CreatePlanet(int planetId, ulong playerId, IReadOnlyCollection<SpaceShip> ships)
        {
            return new Planet(planetId, playerId, ships);
        }
    }
}