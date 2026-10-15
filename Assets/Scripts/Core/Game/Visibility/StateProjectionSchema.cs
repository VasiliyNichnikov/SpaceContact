using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System;
using Newtonsoft.Json.Serialization;

namespace Core.Game.Visibility
{
    internal static class StateProjectionSchema
    {
        private static readonly DefaultContractResolver _contracts = new();
        private static readonly ConcurrentDictionary<Type, StateProjectionMember[]> _members = new();

        public static IReadOnlyList<StateProjectionMember> GetMembers(Type type)
        {
            return _members.GetOrAdd(type, CreateMembers);
        }

        private static StateProjectionMember[] CreateMembers(Type type)
        {
            var resolvedContract = _contracts.ResolveContract(type);

            if (resolvedContract is not JsonObjectContract contract)
            {
                throw new InvalidOperationException($"Unsupported state projection type: {type}.");
            }

            var result = new List<StateProjectionMember>();

            foreach (var property in contract.Properties.Where(property => !property.Ignored && property.Readable))
            {
                var name = property.PropertyName;
                var valueType = property.PropertyType;

                if (name is null || valueType is null)
                {
                    throw new InvalidOperationException($"Incomplete serialization contract for {type}.");
                }

                var visibility = property.AttributeProvider?
                    .GetAttributes(typeof(StateVisibilityAttribute), true)
                    .OfType<StateVisibilityAttribute>()
                    .SingleOrDefault()
                    ?? throw new InvalidOperationException($"Serialized member has no visibility: {type}.{name}.");

                if (!Enum.IsDefined(typeof(StateAudience), visibility.Audience))
                {
                    throw new InvalidOperationException($"Unknown audience on {type}.{name}.");
                }

                if (visibility.IsOwnerId && (valueType != typeof(ulong) || visibility.Audience != StateAudience.Everyone))
                {
                    throw new InvalidOperationException($"Owner ID must be a public ulong: {type}.{name}.");
                }

                if (visibility.Unordered &&
                    (!valueType.IsGenericType || valueType.GetGenericTypeDefinition() != typeof(List<>)))
                {
                    throw new InvalidOperationException($"Unordered visibility requires a list: {type}.{name}.");
                }

                result.Add(new StateProjectionMember(name, valueType, visibility));
            }

            if (result.Count(member => member.Visibility.IsOwnerId) > 1)
            {
                throw new InvalidOperationException($"Multiple owner IDs on {type}.");
            }

            return result.ToArray();
        }
    }
}
