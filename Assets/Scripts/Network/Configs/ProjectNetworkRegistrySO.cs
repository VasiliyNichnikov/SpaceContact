using Network.Requests;
using UnityEngine;

namespace Network.Configs
{
    [CreateAssetMenu(fileName = "ProjectNetworkRegistrySO", menuName = "Configs/Network/ProjectNetworkRegistrySO", order = 0)]
    public class ProjectNetworkRegistrySO : ScriptableObject
    {
        [SerializeField]
        private NetworkServiceObj _networkServiceObj = null!;
        
        public NetworkServiceObj NetworkServiceObj => 
            _networkServiceObj;
    }
}