using System;
using System.Collections.Generic;
using System.Linq;
using Core.Game.Dto.Rules;
using Core.Game.Dto.States;
using Core.Game.Planets;
using Core.Game.Players;
using Core.Game.Players.Visitors;
using Core.User;

namespace Core.Game.Galaxy
{
    public class GameGalaxyManager : IGalaxyManagerNetwork
    {
        private readonly ClientUsersRepository _usersRepository;
        private readonly GamePlayersRegistry _gamePlayersRegistry;
        private readonly RulesOfPlanetsData _rulesOfPlanets;
        
        // key - planetId, value - planet 
        private readonly Dictionary<int, Planet> _planetById = new();
        
        public GameGalaxyManager(
            ClientUsersRepository usersRepository,
            GamePlayersRegistry gamePlayersRegistry,
            RulesOfPlanetsData rulesOfPlanets)
        {
            _usersRepository = usersRepository;
            _gamePlayersRegistry = gamePlayersRegistry;
            InitPlanetsAndShips(rulesOfPlanets);
        }
        
        public event Action? OnStateChanged;

        public void ServerGalaxyLoaded() => 
            UpdatePlayersPlanets();

        void IGalaxyManagerNetwork.ApplyStateData(GalaxyStateData data)
        {
            RefreshGalaxyData(data);
            UpdatePlayersPlanets();
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

        private void UpdatePlayersPlanets()
        {
            foreach (var player in _gamePlayersRegistry.Players)
            {
                var planetsUploaderVisitor = new GamePlayerPlanetsUpdaterVisitor(GetPlayerPlanets);
                player.Apply(planetsUploaderVisitor);
            }
        }

        private void InitPlanetsAndShips(RulesOfPlanetsData rules)
        {
            _planetById.Clear();
            
            var shipId = 0;
            var planetId = 0;

            var sortedPlayers = _usersRepository
                .Users
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
        
        private IReadOnlyCollection<IPlanet> GetPlayerPlanets(ulong playerId)
        {
            var planets = _planetById.Values
                .Where(planet => planet.OwnerId == playerId)
                .ToList();

            return planets;
        }
        
        private void RefreshGalaxyData(GalaxyStateData data)
        {
            foreach (var planetData in data.Planets)
            {
                if (_planetById.TryGetValue(planetData.PlanetId, out var planet))
                {
                    planet.UpdateState(planetData);
                }
                else
                {
                    var ships = planetData
                        .Ships
                        .Select(ship => new SpaceShip(ship.ShipId, ship.OwnerId))
                        .ToList();
                    _planetById[planetData.PlanetId] = CreatePlanet(planetData.PlanetId, planetData.OwnerId, ships);
                }
            }
        }
        
        private static Planet CreatePlanet(int planetId, ulong playerId, IReadOnlyCollection<SpaceShip> ships)
        {
            return new Planet(planetId, playerId, ships);
        }
    }
}