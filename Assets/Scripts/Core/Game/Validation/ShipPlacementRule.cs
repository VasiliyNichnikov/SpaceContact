using System.Collections.Generic;
using Core.Game.Dto.Snapshots;

namespace Core.Game.Validation
{
    public sealed class ShipPlacementRule : StateValidationRule<GalaxySnapshot>
    {
        protected override void Validate(GalaxySnapshot value, StateValidationContext context)
        {
            var placed = new HashSet<int>();
            foreach (var (playerId, planets) in value.Planets)
            {
                for (var planetIndex = 0; planetIndex < planets.Count; planetIndex++)
                {
                    var shipIds = planets[planetIndex].ShipIds;
                    for (var shipIndex = 0; shipIndex < shipIds.Count; shipIndex++)
                    {
                        context.At($".Planets[{playerId}][{planetIndex}].ShipIds[{shipIndex}]")
                            .Require(placed.Add(shipIds[shipIndex]), "A ship cannot be placed on more than one planet.");
                    }
                }
            }
        }
    }
}
