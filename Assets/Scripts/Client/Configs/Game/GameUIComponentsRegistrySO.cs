using Client.UI.Dialogs.Game.PlayerChoice;
using UnityEngine;

namespace Client.Configs.Game
{
    [CreateAssetMenu(fileName = "GameUIComponentsRegistrySO", menuName = "Configs/UI/GameUIComponentsRegistrySO", order = 0)]
    public class GameUIComponentsRegistrySO : ScriptableObject
    {
        [SerializeField]
        private GameArrowsHolder _arrowsHolder = null!;
        
        public GameArrowsHolder ArrowsHolder => 
            _arrowsHolder;
    }
}