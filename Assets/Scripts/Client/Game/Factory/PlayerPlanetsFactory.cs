using Client.Configs.Game;
using Client.Game.Planets;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Client.Game.Factory
{
    public class PlayerPlanetsFactory
    {
        private readonly IObjectResolver _resolver;
        private readonly CosmicPrefabStore _cosmicPrefabStore;
        
        public PlayerPlanetsFactory(
            IObjectResolver resolver, 
            CosmicPrefabStore prefabStore)
        {
            _resolver = resolver;
            _cosmicPrefabStore = prefabStore;
        }

        public PlanetView CreatePlanet(Vector3 position)
        {
            return _resolver.Instantiate(_cosmicPrefabStore.PlanetPrefab, position, Quaternion.identity);
        }
    }
}