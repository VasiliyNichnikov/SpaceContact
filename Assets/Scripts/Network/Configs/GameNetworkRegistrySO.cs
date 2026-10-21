using Network.Game;
using UnityEngine;

namespace Network.Configs
{
    [CreateAssetMenu(fileName = "GameNetworkRegistrySO", menuName = "Configs/Network/GameNetworkRegistrySO", order = 0)]
    public class GameNetworkRegistrySO : ScriptableObject
    {
        [SerializeField]
        private GameEventRpcRelayNetwork _eventRpcRelayNetwork = null!;
        
        public GameEventRpcRelayNetwork EventRpcRelayNetwork =>
            _eventRpcRelayNetwork;
    }
}