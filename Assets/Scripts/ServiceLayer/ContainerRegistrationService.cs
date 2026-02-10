using System.Collections.Generic;
using VContainer;

namespace ServiceLayer
{
    public class ContainerRegistrationService
    {
        private Dictionary<ContainerType, IObjectResolver>? _registrations;
        
        public bool TryGetResolver(ContainerType containerType, out IObjectResolver? resolver)
        {
            if (_registrations != null)
            {
                return _registrations.TryGetValue(containerType, out resolver);
            }
            
            resolver = null;
                
            return false;

        }
        
        public void Register(ContainerType containerType, IObjectResolver resolver)
        {
            _registrations ??= new Dictionary<ContainerType, IObjectResolver>();
            _registrations.Add(containerType, resolver);
        }

        public void Unregister(ContainerType containerType)
        {
            _registrations?.Remove(containerType);
        }
    }
}