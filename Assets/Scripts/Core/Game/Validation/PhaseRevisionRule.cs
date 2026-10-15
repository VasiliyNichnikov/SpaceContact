using Core.Game.Dto.Snapshots;

namespace Core.Game.Validation
{
    public sealed class PhaseRevisionRule : StateValidationRule<PhaseSnapshot>
    {
        protected override void Validate(PhaseSnapshot value, StateValidationContext context)
        {
            context.At(".Revision").Require(value.Revision <= context.Revision, "Phase revision exceeds snapshot revision.");
        }
    }
}
