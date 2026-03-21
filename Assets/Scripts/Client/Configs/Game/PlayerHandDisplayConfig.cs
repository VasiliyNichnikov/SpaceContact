using Client.Data.Game;
using UnityEngine;

namespace Client.Configs.Game
{
    [CreateAssetMenu(fileName = "PlayerHandDisplayConfig", menuName = "Configs/Game/PlayerHandDisplayConfig", order = 0)]
    public class PlayerHandDisplayConfig : ScriptableObject
    {
        [SerializeField]
        private float _archWidth = 600f;
        
        [SerializeField]
        private float _archHeight = 20f;

        [SerializeField]
        private float _maxRotation = 15f;

        public PlayerHandDisplayData BuildData()
        {
            return new PlayerHandDisplayData(
                _archWidth, 
                _archHeight, 
                _maxRotation);
        }
    }
}