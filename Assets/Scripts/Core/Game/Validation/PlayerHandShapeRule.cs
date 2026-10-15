using Core.Game.Dto.Snapshots;

namespace Core.Game.Validation
{
    public sealed class PlayerHandShapeRule : StateValidationRule<PlayerHandSnapshot>
    {
        protected override void Validate(PlayerHandSnapshot value, StateValidationContext context)
        {
            context.At(".Count").Require(value.HasConsistentCount, "The declared hand count disagrees with its cards.");
        }
    }
}
