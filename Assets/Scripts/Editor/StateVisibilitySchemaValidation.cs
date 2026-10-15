#if UNITY_EDITOR
using System.Linq;
using System;
using Core.Game.Dto.Snapshots;
using Core.Game.Events;
using Core.Game.Visibility;

namespace Editor
{
    public sealed class StateVisibilitySchemaValidation : IGameRulesValidation
    {
        public void Validate(GameRulesValidationContext context)
        {
            var types = typeof(IGameEvent).Assembly.GetTypes()
                .Where(type => !type.IsAbstract && typeof(IGameEvent).IsAssignableFrom(type))
                .Append(typeof(GameSnapshot));

            foreach (var type in types)
            {
                try
                {
                    StateProjection.ValidateSchema(type);
                }
                catch (Exception exception)
                {
                    context.AddError($"State visibility schema {type.FullName}: {exception.Message}");
                }
            }
        }
    }
}
#endif
