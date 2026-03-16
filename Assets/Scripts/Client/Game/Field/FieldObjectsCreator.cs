using System.Linq;
using Client.Data.Game;
using Client.Game.Factory;
using Client.Game.Planets.ViewModels;
using Core.Game;
using Core.Game.Planets;
using Core.Game.Players;
using Logs;

namespace Client.Game.Field
{
    public class FieldObjectsCreator
    {
        private readonly PlayerPlanetsFactory _factory;
        private readonly IGameFieldManager _fieldManager;
        private readonly PlanetLayoutSetData _planetLayoutSetData;
        
        public FieldObjectsCreator(
            PlayerPlanetsFactory factory,
            IGameFieldManager fieldManager,
            PlanetLayoutSetData planetLayoutSetData)
        {
            _factory = factory;
            _fieldManager = fieldManager;
            _planetLayoutSetData = planetLayoutSetData;
        }
        
        public void InitPlanets()
        {
            var currentPlayer = _fieldManager.CurrentPlayer;
            CreatePlanets(currentPlayer, currentPlayer.Planets.ToArray(), true);
            
            var oppositePlayer = _fieldManager.OpponentPlayer;
            CreatePlanets(oppositePlayer, oppositePlayer.Planets.ToArray(), false);
        }

        private void CreatePlanets(IGamePlayer player, IPlanet[] planets, bool isCurrentPlayer)
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
                createdPlanet.Init(new PlanetViewModel(player, planetCore));
            }
        }
    }
}