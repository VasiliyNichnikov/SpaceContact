using Core.Game.Dto.Snapshots;
using Core.Helpers;

namespace Core.Game.Validation
{
    public sealed class PhaseRevisionRule : StateValidationRule<PhaseSnapshot>
    {
        protected override void Validate(PhaseSnapshot value, StateValidationContext context)
        {
            context.At(".EndTimeInSeconds").Require(!value.EndTimeInSeconds.HasValue ||
                TransitionPhaseHelper.IsTimedPhase(value.CurrentPhase), "An untimed phase cannot have a deadline.");
            context.At(".ReadyTransitionTimeInSeconds").Require(!value.ReadyTransitionTimeInSeconds.HasValue ||
                value.EndTimeInSeconds.HasValue,
                "A ready deadline requires a phase deadline.");
            context.At(".Revision").Require(value.Revision <= context.Revision, "Phase revision exceeds snapshot revision.");
        }
    }
}
