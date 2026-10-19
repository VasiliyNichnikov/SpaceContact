using System;
using System.Collections.Generic;
using Core.Game.Actions;
using Core.Game.Events;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Network.Infrastructure
{
    public sealed class GameNetworkSerializationBinder : ISerializationBinder
    {
        private static readonly string _assemblyName = typeof(BaseGameAction).Assembly.GetName().Name
            ?? throw new InvalidOperationException("The Core assembly must have a name.");
        private static readonly string _assemblyFullName = typeof(BaseGameAction).Assembly.FullName
            ?? throw new InvalidOperationException("The Core assembly must have a full name.");
        private static readonly IReadOnlyDictionary<string, Type> _typesByName = DiscoverTypes();

        public Type BindToType(string? assemblyName, string typeName)
        {
            if ((string.Equals(assemblyName, _assemblyName, StringComparison.Ordinal) || string.Equals(assemblyName, _assemblyFullName, StringComparison.Ordinal)) && _typesByName.TryGetValue(typeName, out var type))
            {
                return type;
            }

            throw new JsonSerializationException($"Network type '{typeName}, {assemblyName}' is not allowed.");
        }

        public void BindToName(Type serializedType, out string? assemblyName, out string? typeName)
        {
            var fullName = serializedType.FullName;
            
            if (fullName is null || !_typesByName.TryGetValue(fullName, out var type) || type != serializedType)
            {
                throw new JsonSerializationException($"Network type '{serializedType}' is not allowed.");
            }

            assemblyName = _assemblyName;
            typeName = fullName;
        }

        private static IReadOnlyDictionary<string, Type> DiscoverTypes()
        {
            var typesByName = new Dictionary<string, Type>(StringComparer.Ordinal);
            
            foreach (var type in typeof(BaseGameAction).Assembly.GetTypes())
            {
                if (type.IsAbstract || type.IsInterface || type.ContainsGenericParameters)
                {
                    continue;
                }
                
                if(!typeof(BaseGameAction).IsAssignableFrom(type) && !typeof(IGameEvent).IsAssignableFrom(type))
                    continue;

                var fullName = type.FullName ?? throw new InvalidOperationException("A network type must have a full name.");
                typesByName.Add(fullName, type);
            }

            return typesByName;
        }
    }
}
