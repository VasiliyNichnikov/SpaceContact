using Core.Game.Dto.Snapshots;

namespace Core.Game.Validation
{
    public sealed class DestinyCardShapeRule : StateValidationRule<DestinyCardSnapshot>
    {
        protected override void Validate(DestinyCardSnapshot value, StateValidationContext context)
        {
            var forms = (value.IsJoker ? 1 : 0) + (value.SelectedPlayerId.HasValue ? 1 : 0) + (value.SpecificCardId.HasValue ? 1 : 0);
            context.Require(forms == 1, "A Destiny card must have exactly one form.");
        }
    }
}
