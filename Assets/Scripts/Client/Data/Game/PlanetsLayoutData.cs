using UnityEngine;

namespace Client.Data.Game
{
    public readonly struct PlanetsLayoutData
    {
        public readonly Vector3[] PlanetPositions;

        public PlanetsLayoutData(Vector3[] planetPositions)
        {
            PlanetPositions = planetPositions;
        }
    }
}