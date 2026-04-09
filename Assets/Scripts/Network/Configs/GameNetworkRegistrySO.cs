using Network.Game;
using Network.Game.Mutation;
using UnityEngine;

namespace Network.Configs
{
    [CreateAssetMenu(fileName = "GameNetworkRegistrySO", menuName = "Configs/Network/GameNetworkRegistrySO", order = 0)]
    public class GameNetworkRegistrySO : ScriptableObject
    {
        [SerializeField]
        private GalaxyNetworkSync _galaxyNetworkSync = null!;
        
        [SerializeField]
        private GamePlayerNetworkSync _playerNetworkSync = null!;
        
        [SerializeField]
        private GameStatesNetworkSync _statesNetworkSync = null!;
        
        [SerializeField]
        private GameEventRpcRelayNetwork _eventRpcRelayNetwork = null!;
        
        public GalaxyNetworkSync GalaxyNetworkSync => 
            _galaxyNetworkSync;
        
        public GamePlayerNetworkSync PlayerNetworkSync => 
            _playerNetworkSync;
        
        public GameStatesNetworkSync StatesNetworkSync =>
            _statesNetworkSync;
        
        public GameEventRpcRelayNetwork EventRpcRelayNetwork =>
            _eventRpcRelayNetwork;
    }
}