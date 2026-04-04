using System;
using System.Collections.Generic;
using System.Linq;
using Client.Data.Game;
using UnityEngine;

namespace Client.Configs.Game
{
    [CreateAssetMenu(fileName = "CosmicFieldConfig", menuName = "Configs/Game/CosmicFieldConfig", order = 0)]
    public class CosmicFieldConfig : ScriptableObject
    {
        [Serializable]
        private struct PlanetPosition
        {
            public string Comment;
            
            public Vector3 PlayerPlanetPosition;
            
            public Vector3 OppositePlanetPosition;
        }
        
        [Serializable]
        private struct DependenceOnNumberOfPlanets
        {
            public string Comment;
            
            public PlanetPosition[] PlanetPositions;
        }
        
        [SerializeField]
        private DependenceOnNumberOfPlanets[] _settings = null!;
        
        [SerializeField, Min(0)]
        private float _distanceBetweenCentralPlanetsByX;
        
        public PlanetLayoutSetData BuildData()
        {
            var playerLayoutsByPlanetCount = new Dictionary<int, PlanetsLayoutData>();
            var oppositeLayoutsByPlanetCount = new Dictionary<int, PlanetsLayoutData>();

            foreach (var dependence in _settings)
            {
                var playerPlanetsPositions = dependence
                    .PlanetPositions
                    .Select(planetPosition => planetPosition.PlayerPlanetPosition)
                    .ToArray();
                var oppositePlanetsPositions = dependence
                    .PlanetPositions
                    .Select(planetPosition => planetPosition.OppositePlanetPosition)
                    .ToArray();
                
                var playerLayout = new PlanetsLayoutData(playerPlanetsPositions);
                var oppositeLayout = new PlanetsLayoutData(oppositePlanetsPositions);

                playerLayoutsByPlanetCount.Add(playerLayout.PlanetPositions.Length, playerLayout);
                oppositeLayoutsByPlanetCount.Add(oppositeLayout.PlanetPositions.Length, oppositeLayout);
            }

            return new PlanetLayoutSetData(
                playerLayoutsByPlanetCount,
                oppositeLayoutsByPlanetCount,
                _distanceBetweenCentralPlanetsByX
            );
        }
    }
}