using Client.Game.Planets;
using UnityEngine;

namespace Client.Configs.Game
{
    [CreateAssetMenu(fileName = "GameUIShipsOnPlanetItemsRegistrySO", menuName = "Configs/UI/GameUIShipsOnPlanetItemsRegistrySO", order = 0)]
    public class GameUIShipsOnPlanetItemsRegistrySO : ScriptableObject
    {
        [SerializeField] 
        private GameShipsInfoItemView _shipsInfoOnPlanetItemView = null!;
        
        public GameShipsInfoItemView ShipsInfoOnPlanetItemView =>
            _shipsInfoOnPlanetItemView;
    }
}