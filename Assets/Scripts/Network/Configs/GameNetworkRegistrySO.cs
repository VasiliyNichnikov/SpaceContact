using Network.Game;
using Network.Game.Mutation;
using UnityEngine;

namespace Network.Configs
{
    [CreateAssetMenu(fileName = "GameNetworkRegistrySO", menuName = "Configs/Network/GameNetworkRegistrySO", order = 0)]
    public class GameNetworkRegistrySO : ScriptableObject
    {
        [SerializeField]
        private GamePlayerNetworkSync _playerNetworkSync = null!;
        
        [SerializeField]
        private GameEventRpcRelayNetwork _eventRpcRelayNetwork = null!;
        
        public GamePlayerNetworkSync PlayerNetworkSync => 
            _playerNetworkSync;
        
        public GameEventRpcRelayNetwork EventRpcRelayNetwork =>
            _eventRpcRelayNetwork;
    }
}