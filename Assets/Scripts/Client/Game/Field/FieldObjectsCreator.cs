using System.Linq;
using Client.Data.Game;
using Client.Game.Factory;
using Client.Game.Planets.ViewModels;
using Core.Game;
using Core.Game.Planets;
using Logs;

namespace Client.Game.Field
{
    public class FieldObjectsCreator
    {
        private readonly PlayerPlanetsFactory _factory;
        private readonly ITwoPlayerFieldManager _twoPlayerFieldManager;
        private readonly PlanetLayoutSetData _planetLayoutSetData;
        
        public FieldObjectsCreator(
            PlayerPlanetsFactory factory, 
            ITwoPlayerFieldManager twoPlayerFieldManager,
            PlanetLayoutSetData planetLayoutSetData)
        {
            _factory = factory;
            _twoPlayerFieldManager = twoPlayerFieldManager;
            _planetLayoutSetData = planetLayoutSetData;
        }
        
        public void InitPlanets()
        {
            var currentPlayer = _twoPlayerFieldManager.CurrentPlayer;
            CreatePlanets(currentPlayer.Planets.ToArray(), true);

            var oppositePlayer = _twoPlayerFieldManager.OpponentPlayer;
            CreatePlanets(oppositePlayer.Planets.ToArray(), false);
        }

        private void CreatePlanets(IPlanet[] planets, bool isCurrentPlayer)
        {
            var layout = isCurrentPlayer 
                ? _planetLayoutSetData.GetPlayerPlanetsLayoutData(planets.Length)
                : _planetLayoutSetData.GetOppositePlanetsLayoutData(planets.Length);

            if (layout.PlanetPositions.Length != planets.Length)
            {
                Logger.Error("FieldObjectsCreator.CreatePlanets: planets count != planets length.");
                
                return;
            }

            for (var i = 0; i < planets.Length; i++)
            {
                var planetCore = planets[i];
                var planetPosition = layout.PlanetPositions[i];
                
                var createdPlanet = _factory.CreatePlanet(planetPosition);
                createdPlanet.Init(new PlanetViewModel(planetCore));
            }
        }
    }
}