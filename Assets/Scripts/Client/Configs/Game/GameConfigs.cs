using UnityEngine;
using VContainer;

namespace Client.Configs.Game
{
    [CreateAssetMenu(fileName = "GameConfigs", menuName = "Configs/Game/GameConfigs", order = 0)]
    public class GameConfigs : ScriptableObject
    {
        [SerializeField]
        private RulesOfPlanetsConfig _rulesOfPlanetsConfig = null!;
        
        [SerializeField]
        private CosmicFieldConfig _cosmicFieldConfig = null!;
        
        [SerializeField]
        private CosmicPrefabStore _cosmicPrefabStore = null!;
        
        public void Build(IContainerBuilder builder)
        {
            builder.RegisterInstance(_rulesOfPlanetsConfig.BuildData());
            builder.RegisterInstance(_cosmicFieldConfig.BuildData());
            builder.RegisterInstance(_cosmicPrefabStore);
        }
    }
}