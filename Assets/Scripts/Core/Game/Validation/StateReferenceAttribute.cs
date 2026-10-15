using System.Collections.Generic;
using System.Collections;
using System.Globalization;
using System;

namespace Core.Game.Validation
{
    public sealed class StateReferenceAttribute : StateValidationAttribute
    {
        public StateReferenceAttribute(Type entityType)
        {
            EntityType = entityType;
        }

        public Type EntityType { get; }

        public bool MatchAllIdentities { get; set; }

        public override void Validate(object? value, StateValidationContext context)
        {
            if (value is null)
            {
                return;
            }

            var keys = value is IDictionary dictionary ? dictionary.Keys : value as IEnumerable;
            if (keys is null || value is string)
            {
                context.RequireReference(EntityType, value);
                return;
            }

            var ids = new HashSet<object>();
            var index = 0;
            foreach (object? key in keys)
            {
                var location = value is IDictionary && key is ulong playerId
                    ? playerId.ToString(CultureInfo.InvariantCulture)
                    : index.ToString(CultureInfo.InvariantCulture);
                var itemContext = context.At($"[{location}]");
                index++;
                if (key is null)
                {
                    throw new StateValidationException(itemContext.Path, "A reference cannot be null.");
                }

                ids.Add(key);
                itemContext.RequireReference(EntityType, key);
            }

            if (MatchAllIdentities)
            {
                context.RequireAllIdentities(EntityType, ids);
            }
        }
    }
}
