using Client.Game.Planets;
using UnityEngine;

namespace Client.Configs.Game
{
    [CreateAssetMenu(fileName = "CosmicPrefabStore", menuName = "Configs/Game/CosmicPrefabStore", order = 0)]
    public class CosmicPrefabStore : ScriptableObject
    {
        [SerializeField] 
        private PlanetView _planetPrefab = null!;
        
        public PlanetView PlanetPrefab => _planetPrefab;
    }
}