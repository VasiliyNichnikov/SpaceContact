using Network.Game;
using UnityEngine;

namespace Network.Configs
{
    [CreateAssetMenu(fileName = "GameNetworkRegistrySO", menuName = "Configs/Network/GameNetworkRegistrySO", order = 0)]
    public class GameNetworkRegistrySO : ScriptableObject
    {
        [SerializeField]
        private GalaxyNetworkSync _galaxyNetworkSync = null!;
        
        [SerializeField]
        private GamePlayerNetworkSync _gamePlayerNetworkSync = null!;
        
        public GalaxyNetworkSync GalaxyNetworkSync => 
            _galaxyNetworkSync;
        
        public GamePlayerNetworkSync GamePlayerNetworkSync => 
            _gamePlayerNetworkSync;
    }
}