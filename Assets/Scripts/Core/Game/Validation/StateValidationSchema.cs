using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System;
using Core.Game.Visibility;
using Newtonsoft.Json.Serialization;

namespace Core.Game.Validation
{
    internal static class StateValidationSchema
    {
        private static readonly DefaultContractResolver _contracts = new();
        private static readonly ConcurrentDictionary<Type, StateValidationMember[]> _members = new();
        private static readonly ConcurrentDictionary<Type, IStateValidationRule[]> _rules = new();

        public static IReadOnlyList<StateValidationMember> GetMembers(Type type)
        {
            return _members.GetOrAdd(type, CreateMembers);
        }

        public static IReadOnlyList<IStateValidationRule> GetRules(Type type)
        {
            return _rules.GetOrAdd(type, CreateRules);
        }

        private static StateValidationMember[] CreateMembers(Type type)
        {
            if (_contracts.ResolveContract(type) is not JsonObjectContract contract)
            {
                throw new InvalidOperationException($"Unsupported validation type: {type}.");
            }

            return contract.Properties
                .Where(property => !property.Ignored && property.Readable)
                .Select(property => CreateValidationMember(type, property))
                .ToArray();
        }

        private static StateValidationMember CreateValidationMember(Type type, JsonProperty property)
        {
            var name = property.PropertyName ?? throw new InvalidOperationException("Missing JSON member name.");
            var valueType = property.PropertyType ?? throw new InvalidOperationException("Missing JSON member type.");
            var provider = property.ValueProvider ?? throw new InvalidOperationException("Missing JSON value provider.");
            var attributes = property.AttributeProvider?.GetAttributes(true).ToArray() ?? Array.Empty<Attribute>();
            var visibility = attributes.OfType<StateVisibilityAttribute>().SingleOrDefault() ?? throw new InvalidOperationException($"Missing visibility on {type}.{name}.");

            var checks = attributes.OfType<StateValidationAttribute>().ToArray();

            return new StateValidationMember(name, valueType, provider, visibility, checks);
        }

        private static IStateValidationRule[] CreateRules(Type type)
        {
            return type.GetCustomAttributes<StateRuleAttribute>().Select(attribute =>
            {
                if (!typeof(IStateValidationRule).IsAssignableFrom(attribute.RuleType) || attribute.RuleType.IsAbstract)
                {
                    throw new InvalidOperationException($"Invalid state rule on {type}.");
                }

                return (IStateValidationRule)(Activator.CreateInstance(attribute.RuleType)
                    ?? throw new InvalidOperationException($"Cannot create state rule on {type}."));
            }).ToArray();
        }
    }
}
