using System.Linq;
using Core.Game.Dto.Snapshots;

namespace Core.Game.Validation
{
    public sealed class EncounterDependenciesRule : StateValidationRule<EncounterSnapshot>
    {
        protected override void Validate(EncounterSnapshot value, StateValidationContext context)
        {
            context.At(".DefenderPlayerId").Require(!value.DefenderPlayerId.HasValue || value.AggressorPlayerId.HasValue,
                "A defender requires an aggressor.");
            context.At(".PlanetIdToAttack").Require(!value.PlanetIdToAttack.HasValue || value.DefenderPlayerId.HasValue,
                "A target requires a defender.");
            context.At(".AggressorUnionPlayerIds").Require(value.AggressorUnionPlayerIds.Count == 0 || value.AggressorPlayerId.HasValue,
                "An attacking union requires an aggressor.");
            context.At(".DefenderUnionPlayerIds").Require(value.DefenderUnionPlayerIds.Count == 0 || value.DefenderPlayerId.HasValue,
                "A defending union requires a defender.");
            context.At(".DefenderUnionPlayerIds").Require(!value.AggressorUnionPlayerIds.Intersect(value.DefenderUnionPlayerIds).Any(),
                "A player cannot belong to both unions.");
        }
    }
}
