using System.Collections.Generic;
using System.Linq;
using Core.Game.Dto.Rules;
using Core.Game.Dto.States;
using Core.Game.Players;

namespace Core.Game.Galaxy.Server
{
    public sealed class GameServerGalaxyManager : IGameServerGalaxyManager
    {
        private readonly GamePlayersRegistry _gamePlayersRegistry;
        private readonly RulesOfPlanetsData _rulesOfPlanets;
        
        // key - planetId, value - planet 
        private readonly Dictionary<int, PlanetStateData> _planetById = new();

        public GameServerGalaxyManager(
            GamePlayersRegistry gamePlayersRegistry,
            RulesOfPlanetsData rulesOfPlanets)
        {
            _gamePlayersRegistry = gamePlayersRegistry;
            _rulesOfPlanets = rulesOfPlanets;
        }
        
        void IGameServerGalaxyManager.Init() => 
            InitPlanetsAndShips(_rulesOfPlanets);

        GalaxyStateData IGameServerGalaxyManager.ToState()
        {
            var state = new GalaxyStateData();
            state.Planets.AddRange(_planetById.Values);
            
            return state;
        }
        
        private void InitPlanetsAndShips(RulesOfPlanetsData rules)
        {
            _planetById.Clear();
            
            var shipId = 0;
            var planetId = 0;

            var sortedPlayers = _gamePlayersRegistry
                .Players
                .OrderBy(p => p.Order);
            
            var shipsByPlayer = new Dictionary<ulong, List<SpaceShipStateData>>();
            
            foreach (var player in sortedPlayers)
            {
                for (var i = 0; i < rules.NumberOfShipsOnPlanet; i++)
                {
                    var createdShip = new SpaceShipStateData
                    {
                        ShipId = shipId,
                        OwnerId = player.PlayerId,
                    };
                    
                    if (shipsByPlayer.TryGetValue(player.PlayerId, out var ships))
                    {
                        ships.Add(createdShip);
                    }
                    else
                    {
                        shipsByPlayer.Add(player.PlayerId, new List<SpaceShipStateData> { createdShip });
                    }
                    
                    shipId++;
                }
                
                for (var i = 0; i < rules.NumberOfPlanetsPlayer; i++)
                {
                    var shipsOnPlanet = shipsByPlayer[player.PlayerId];
                    var createdPlanet = new PlanetStateData
                    {
                        PlanetId = planetId,
                        OwnerId = player.PlayerId,
                        Ships = shipsOnPlanet
                    };
                    _planetById[planetId] = createdPlanet;
                    planetId++;
                }
            }
        }
    }
}