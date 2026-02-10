using System.Collections.Generic;

namespace Client.Data.Game
{
    public readonly struct PlanetLayoutSetData
    {
        private readonly Dictionary<int, PlanetsLayoutData> _playerLayoutsByPlanetCount;
        private readonly Dictionary<int, PlanetsLayoutData> _oppositeLayoutsByPlanetCount;

        public PlanetLayoutSetData(
            Dictionary<int, PlanetsLayoutData> playerLayoutsByPlanetCount,
            Dictionary<int, PlanetsLayoutData> oppositeLayoutsByPlanetCount)
        {
            _playerLayoutsByPlanetCount = playerLayoutsByPlanetCount;
            _oppositeLayoutsByPlanetCount = oppositeLayoutsByPlanetCount;
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