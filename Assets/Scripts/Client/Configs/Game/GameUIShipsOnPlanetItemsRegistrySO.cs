using Client.Game.Planets;
using UnityEngine;

namespace Client.Configs.Game
{
    [CreateAssetMenu(fileName = "GameUIShipsOnPlanetItemsRegistrySO", menuName = "Configs/UI/GameUIShipsOnPlanetItemsRegistrySO", order = 0)]
    public class GameUIShipsOnPlanetItemsRegistrySO : ScriptableObject
    {
        [SerializeField] 
        private GameShipsInfoItemView _shipsInfoOnPlanetItemView = null!;
        
        [SerializeField]
        private GameChoicePlanetToAttackItemView _choicePlanetToAttackItemView = null!;
        
        public GameShipsInfoItemView ShipsInfoOnPlanetItemView =>
            _shipsInfoOnPlanetItemView;

        public GameChoicePlanetToAttackItemView ChoicePlanetToAttackItemView => 
            _choicePlanetToAttackItemView;
    }
}