using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;
using Newtonsoft.Json.Serialization;

namespace Core.Game.Validation
{
    public sealed class StateUniqueAttribute : StateValidationAttribute
    {
        private static readonly DefaultContractResolver _contracts = new();

        public StateUniqueAttribute(string? memberName = null)
        {
            MemberName = memberName;
        }

        public string? MemberName { get; }

        public override void Validate(object? value, StateValidationContext context)
        {
            if (value is null)
            {
                return;
            }

            if (value is not IEnumerable items || value is string)
            {
                throw new InvalidOperationException("StateUnique requires a collection.");
            }

            var seen = new HashSet<object>();
            var index = 0;
            foreach (object? item in items)
            {
                var itemContext = context.At($"[{index++}]");
                object? key = item;
                if (item is not null && MemberName is not null)
                {
                    var contract = _contracts.ResolveContract(item.GetType()) as JsonObjectContract;
                    var member = contract?.Properties.SingleOrDefault(property => property.UnderlyingName == MemberName);
                    var provider = member?.ValueProvider
                        ?? throw new InvalidOperationException("StateUnique refers to an unknown member.");
                    key = provider.GetValue(item);
                }

                itemContext.Require(key is not null && seen.Add(key), "Collection keys must be non-null and unique.");
            }
        }
    }
}
