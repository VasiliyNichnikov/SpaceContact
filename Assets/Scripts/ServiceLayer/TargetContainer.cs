using UnityEngine;

namespace ServiceLayer
{
    public class TargetContainer : MonoBehaviour
    {
        [SerializeField]
        private ContainerType _containerType;
        
        public ContainerType ContainerType => 
            _containerType;
    }
}