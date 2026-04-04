using Client.Game.Planets;
using Client.UI.Dialogs.Game.PlayerChoice;
using UnityEngine;

namespace Client.Configs.Game
{
    [CreateAssetMenu(fileName = "GameUIComponentsRegistrySO", menuName = "Configs/UI/GameUIComponentsRegistrySO", order = 0)]
    public class GameUIComponentsRegistrySO : ScriptableObject
    {
        [SerializeField]
        private GameArrowsHolder _arrowsHolder = null!;

        [SerializeField] 
        private GameShipsOnPlanetInfoView _shipsOnPlanetInfoView = null!;
        
        public GameArrowsHolder ArrowsHolder => 
            _arrowsHolder;
        
        public GameShipsOnPlanetInfoView ShipsOnPlanetInfoView =>
            _shipsOnPlanetInfoView;
    }
}