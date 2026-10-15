#if UNITY_EDITOR
using System;
using Core.Game.Dto.Snapshots;
using Core.Game.Validation;

namespace Editor
{
    public sealed class SnapshotValidationSchemaValidation : IGameRulesValidation
    {
        public void Validate(GameRulesValidationContext context)
        {
            try
            {
                StateValidator.ValidateSchema(typeof(GameSnapshot));
            }
            catch (Exception exception)
            {
                context.AddError($"Snapshot validation schema: {exception.Message}");
            }
        }
    }
}
#endif
