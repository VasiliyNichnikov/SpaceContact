using Client.UI.HUDs;
using UnityEngine;

namespace Client.Configs.Game
{
    [CreateAssetMenu(fileName = "HUDsRegistrySO", menuName = "Configs/UI/HUDsRegistrySO", order = 0)]
    public class HUDsRegistrySO : ScriptableObject
    {
        [SerializeField] 
        private GameHUD _gameHud = null!;
        
        public GameHUD GameHud => _gameHud;
    }
}