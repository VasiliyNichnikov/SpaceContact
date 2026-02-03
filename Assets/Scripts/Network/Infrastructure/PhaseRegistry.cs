using System;
using System.Collections.Generic;
using Core.Phases;

namespace Network.Infrastructure
{
    public class PhaseRegistry
    {
        private readonly Dictionary<Type, byte> _typeToId = new();
        
        private readonly Dictionary<byte, Type> _idToPhaseType = new();
        
        private readonly Dictionary<Type, Type> _phaseToDataType = new();

        public void RegisterPhase<TPhase>(byte id) where TPhase : IGamePhase
        {
            var phaseType = typeof(TPhase);
            _idToPhaseType.Add(id, phaseType);
            _typeToId.Add(phaseType, id);

            foreach (var @interface in phaseType.GetInterfaces())
            {
                if (@interface.IsGenericType 
                    && @interface.GetGenericTypeDefinition() == typeof(IPhaseWithContext<>))
                {
                    _phaseToDataType.Add(typeof(TPhase), @interface.GenericTypeArguments[0]);
                    break;
                }
            }
        }

        public byte GetId<TPhase>() where TPhase : IGamePhase
        {
            if (!_typeToId.TryGetValue(typeof(TPhase), out var id))
            {
                throw new Exception($"Phase ID \"{id}\" is unknown! Check your RegisterPhase calls.");
            }
            
            return id;
        }

        public Type GetPhaseType(byte id) => 
            _idToPhaseType[id];

        public Type? GetDataType(Type type)
        {
            _phaseToDataType.TryGetValue(type, out var result);
            
            return result;
        }
    }
}