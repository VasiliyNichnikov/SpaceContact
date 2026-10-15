using System.Collections.Generic;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System;
using Core.Game.Visibility;

namespace Core.Game.Validation
{
    public static class StateValidator
    {
        public static void Validate(object? value, GameContextRole role, ulong localPlayerId, long revision)
        {
            if (value is null)
            {
                throw new StateValidationException("$", "A snapshot is required.");
            }

            var session = new StateValidationSession(role, localPlayerId, revision);
            var context = new StateValidationContext(session, "$", null);
            context.Require(role is GameContextRole.Server or GameContextRole.Client, "Unknown context role.");
            context.At(".Revision").Require(revision >= 0, "Revision cannot be negative.");
            Visit(value, context, new HashSet<object>(new ReferenceComparer()));
            session.Complete();
        }

        public static void ValidateSchema(Type type)
        {
            StateProjection.ValidateSchema(type);
            ValidateSchema(type, new HashSet<Type>());
        }

        private static void ValidateSchema(Type type, ISet<Type> visited)
        {
            type = Nullable.GetUnderlyingType(type) ?? type;
            if (!visited.Add(type) || type.IsPrimitive || type.IsEnum || type == typeof(string) || type == typeof(decimal))
            {
                return;
            }

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
            {
                ValidateSchema(type.GetGenericArguments()[1], visited);
                return;
            }

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                ValidateSchema(type.GetGenericArguments()[0], visited);
                return;
            }

            StateValidationSchema.GetRules(type);
            foreach (var member in StateValidationSchema.GetMembers(type))
            {
                ValidateSchema(member.ValueType, visited);
            }
        }

        private static void Visit(object value, StateValidationContext context, ISet<object> ancestors)
        {
            var type = value.GetType();
            if (type.IsPrimitive || type.IsEnum || value is string || value is decimal)
            {
                return;
            }

            context.Require(ancestors.Add(value), "The state graph contains a cycle.");
            try
            {
                if (value is IDictionary dictionary)
                {
                    foreach (DictionaryEntry entry in dictionary)
                    {
                        if (entry.Key is not ulong owner)
                        {
                            throw new StateValidationException(context.Path, "Dictionary keys must be player IDs.");
                        }

                        var itemContext = context.At($"[{owner.ToString(CultureInfo.InvariantCulture)}]").WithOwner(owner);
                        VisitItem(entry.Value, itemContext, ancestors);
                    }

                    return;
                }

                if (value is IList items)
                {
                    for (var i = 0; i < items.Count; i++)
                    {
                        VisitItem(items[i], context.At($"[{i}]"), ancestors);
                    }

                    return;
                }

                var members = StateValidationSchema.GetMembers(type);
                var ownerMember = members.SingleOrDefault(member => member.Visibility.IsOwnerId);
                if (ownerMember is not null)
                {
                    if (ownerMember.ValueProvider.GetValue(value) is not ulong owner)
                    {
                        throw new StateValidationException(context.Path, "An owner ID must be a ulong.");
                    }

                    context.At("." + ownerMember.Name).Require(!context.OwnerPlayerId.HasValue || context.OwnerPlayerId.Value == owner,
                        "Owner ID disagrees with its containing state.");
                    context = context.WithOwner(owner);
                }

                foreach (var member in members)
                {
                    var memberContext = context.At("." + member.Name);
                    var memberValue = member.ValueProvider.GetValue(value);
                    var audience = member.Visibility.Audience;
                    memberContext.Require(Enum.IsDefined(typeof(StateAudience), audience), "Unknown audience.");
                    var visible = context.Role == GameContextRole.Server || audience == StateAudience.Everyone ||
                        (audience == StateAudience.Owner && context.OwnerPlayerId == context.LocalPlayerId);
                    if (!visible)
                    {
                        memberContext.Require(memberValue is null, "The snapshot contains data hidden from this recipient.");
                        continue;
                    }

                    foreach (var check in member.Checks)
                    {
                        check.Validate(memberValue, memberContext);
                    }

                    if (memberValue is not null)
                    {
                        Visit(memberValue, memberContext, ancestors);
                    }
                }

                foreach (var rule in StateValidationSchema.GetRules(type))
                {
                    var ruleContext = context;
                    context.Defer(() => rule.Validate(value, ruleContext));
                }
            }
            finally
            {
                ancestors.Remove(value);
            }
        }

        private static void VisitItem(object? value, StateValidationContext context, ISet<object> ancestors)
        {
            if (value is null)
            {
                throw new StateValidationException(context.Path, "Collection elements cannot be null.");
            }

            Visit(value, context, ancestors);
        }

        private sealed class ReferenceComparer : IEqualityComparer<object>
        {
            public new bool Equals(object? x, object? y)
            {
                return ReferenceEquals(x, y);
            }

            public int GetHashCode(object obj)
            {
                return RuntimeHelpers.GetHashCode(obj);
            }
        }
    }
}
