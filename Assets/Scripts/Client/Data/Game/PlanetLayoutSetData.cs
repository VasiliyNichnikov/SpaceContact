using System.Collections.Generic;

namespace Client.Data.Game
{
    public readonly struct PlanetLayoutSetData
    {
        public readonly float DistanceBetweenCentralPlanetsByX;
        
        private readonly Dictionary<int, PlanetsLayoutData> _playerLayoutsByPlanetCount;
        private readonly Dictionary<int, PlanetsLayoutData> _oppositeLayoutsByPlanetCount;
        
        public PlanetLayoutSetData(
            Dictionary<int, PlanetsLayoutData> playerLayoutsByPlanetCount,
            Dictionary<int, PlanetsLayoutData> oppositeLayoutsByPlanetCount,
            float distanceBetweenCentralPlanetsByX)
        {
            _playerLayoutsByPlanetCount = playerLayoutsByPlanetCount;
            _oppositeLayoutsByPlanetCount = oppositeLayoutsByPlanetCount;
            DistanceBetweenCentralPlanetsByX = distanceBetweenCentralPlanetsByX;
        }

        public PlanetsLayoutData GetPlayerPlanetsLayoutData(int countPlanets)
        {
            return _playerLayoutsByPlanetCount.GetValueOrDefault(countPlanets);
        }

        public PlanetsLayoutData GetOppositePlanetsLayoutData(int countPlanets)
        {
            return _oppositeLayoutsByPlanetCount.GetValueOrDefault(countPlanets);
        }
    }
}