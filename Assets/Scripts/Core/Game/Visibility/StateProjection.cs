using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Core.Game.Visibility
{
    public static class StateProjection
    {
        public static T ToClient<T>(T source, ulong recipientPlayerId, ulong? ownerPlayerId = null) where T : class
        {
            var type = source.GetType();
            ValidateSchema(type);
            var serializer = CreateSerializer();
            var token = JToken.FromObject(source, serializer);
            Filter(token, type, recipientPlayerId, ownerPlayerId);
            var result = (T)(token.ToObject(type, serializer) ?? throw new InvalidOperationException($"Could not materialize projected {type}."));
#if UNITY_EDITOR || DEBUG
            var materialized = JToken.FromObject(result, serializer);
            var permitted = materialized.DeepClone();
            Filter(permitted, type, recipientPlayerId, ownerPlayerId);

            if (!JToken.DeepEquals(materialized, permitted))
            {
                throw new InvalidOperationException($"Defaults on {type} restored hidden data after projection.");
            }
#endif

            return result;
        }

        public static void EnsurePublic<T>(T source, ulong recipientPlayerId) where T : class
        {
            var type = source.GetType();
            ValidateSchema(type);
            var original = JToken.FromObject(source, CreateSerializer());
            var projected = original.DeepClone();
            Filter(projected, type, recipientPlayerId, null);

            if (!JToken.DeepEquals(original, projected))
            {
                throw new InvalidOperationException($"{type} requires an explicit client event projection.");
            }
        }

        public static void ValidateSchema(Type type)
        {
            ValidateSchema(type, new HashSet<Type>());
        }

        private static JsonSerializer CreateSerializer()
        {
            return JsonSerializer.Create(new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.None,
                ReferenceLoopHandling = ReferenceLoopHandling.Error
            });
        }

        private static bool IsScalar(Type type)
        {
            return type.IsPrimitive || type.IsEnum || type == typeof(string) || type == typeof(decimal);
        }

        private static void ValidateSchema(Type type, ISet<Type> visited)
        {
            type = Nullable.GetUnderlyingType(type) ?? type;

            if (!visited.Add(type) || IsScalar(type))
            {
                return;
            }

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
            {
                var arguments = type.GetGenericArguments();

                if (arguments[0] != typeof(ulong))
                {
                    throw new InvalidOperationException($"State dictionary keys must be authoritative player IDs: {type}.");
                }

                ValidateSchema(arguments[1], visited);
                return;
            }

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                ValidateSchema(type.GetGenericArguments()[0], visited);
                return;
            }

            foreach (var member in StateProjectionSchema.GetMembers(type))
            {
                ValidateSchema(member.ValueType, visited);
            }
        }

        private static void Filter(JToken token, Type type, ulong recipientPlayerId, ulong? ownerPlayerId)
        {
            type = Nullable.GetUnderlyingType(type) ?? type;

            if (token.Type == JTokenType.Null || IsScalar(type))
            {
                return;
            }

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
            {
                var valueType = type.GetGenericArguments()[1];

                foreach (var entry in ((JObject)token).Properties())
                {
                    var owner = ulong.Parse(entry.Name, CultureInfo.InvariantCulture);
                    Filter(entry.Value, valueType, recipientPlayerId, owner);
                }

                return;
            }

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                var elementType = type.GetGenericArguments()[0];

                foreach (var element in (JArray)token)
                {
                    Filter(element, elementType, recipientPlayerId, ownerPlayerId);
                }

                return;
            }

            var members = StateProjectionSchema.GetMembers(type);
            var value = (JObject)token;
            var ownerMember = members.SingleOrDefault(member => member.Visibility.IsOwnerId);

            if (ownerMember is not null)
            {
                var ownerToken = value[ownerMember.Name]
                    ?? throw new InvalidOperationException($"Missing owner ID on {type}.");
                var owner = ownerToken.Value<ulong>();

                if (ownerPlayerId.HasValue && ownerPlayerId.Value != owner)
                {
                    throw new InvalidOperationException($"Owner ID disagrees with its containing state: {type}.");
                }

                ownerPlayerId = owner;
            }

            var byName = members.ToDictionary(member => member.Name);

            foreach (var property in value.Properties().ToArray())
            {
                if (!byName.TryGetValue(property.Name, out var member))
                {
                    throw new InvalidOperationException($"Unmapped serialized state: {type}.{property.Name}.");
                }

                var audience = member.Visibility.Audience;

                if (audience == StateAudience.ServerOnly ||
                    (audience == StateAudience.Owner && ownerPlayerId != recipientPlayerId))
                {
                    property.Remove();
                    continue;
                }

                Filter(property.Value, member.ValueType, recipientPlayerId, ownerPlayerId);

                if (member.Visibility.Unordered && property.Value is JArray array)
                {
                    var sorted = array.OrderBy(item => item.ToString(Formatting.None), StringComparer.Ordinal).ToArray();
                    array.RemoveAll();
                    foreach (var item in sorted)
                    {
                        array.Add(item);
                    }
                }
            }
        }
    }
}
