using Core.Game.Dto.Snapshots;

namespace Core.Game.Validation
{
    public sealed class SpaceCardShapeRule : StateValidationRule<SpaceCardSnapshot>
    {
        protected override void Validate(SpaceCardSnapshot value, StateValidationContext context)
        {
            var forms = (value.DamageCount.HasValue ? 1 : 0) + (value.ArtifactId.HasValue ? 1 : 0) + (value.IsConversation ? 1 : 0);
            context.Require(forms == 1, "A space card must have exactly one form.");
        }
    }
}
