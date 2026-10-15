using System.Reflection;
using Core.Game.Visibility;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;

namespace Core.Game.Validation
{
    public sealed class StateValidationContractResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);
            var required = member.IsDefined(typeof(StateRequiredAttribute), true);
            var visibility = member.GetCustomAttribute<StateVisibilityAttribute>();

            // Hidden fields may be omitted. Their requiredness is checked with the recipient at LoadSnapshot.
            if (required && (visibility is null || visibility.Audience == StateAudience.Everyone))
            {
                property.Required = Required.Always;
            }

            return property;
        }
    }
}
