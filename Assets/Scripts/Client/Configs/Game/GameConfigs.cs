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
        
        [SerializeField]
        private CardsConfig _cardsConfig = null!;
        
        [SerializeField]
        private PlayerHandDisplayConfig _handDisplayConfig = null!;
        
        [SerializeField]
        private GameUIComponentsRegistrySO _uiComponentsRegistrySO = null!;
        
        [SerializeField]
        private GameUIShipsOnPlanetItemsRegistrySO _uiShipsOnPlanetItemsRegistrySO;
        
        public void Build(IContainerBuilder builder)
        {
            builder.RegisterInstance(_rulesOfPlanetsConfig.BuildData());
            builder.RegisterInstance(_cosmicFieldConfig.BuildData());
            builder.RegisterInstance(_cardsConfig.BuildData());
            builder.RegisterInstance(_cosmicPrefabStore);
            builder.RegisterInstance(_handDisplayConfig.BuildData());
            builder.RegisterInstance(_uiComponentsRegistrySO);
            builder.RegisterInstance(_uiShipsOnPlanetItemsRegistrySO);
        }
    }
}