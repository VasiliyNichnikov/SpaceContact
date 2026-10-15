using System.Linq;
using Core.Game.Dto.Snapshots;

namespace Core.Game.Validation
{
    public sealed class LocalPlayerSnapshotRule : StateValidationRule<GameSnapshot>
    {
        protected override void Validate(GameSnapshot value, StateValidationContext context)
        {
            if (context.Role == GameContextRole.Client)
            {
                context.At(".GamePlayers.Players").Require(value.GamePlayers.Players.Any(player => player.PlayerId == context.LocalPlayerId),
                    "The snapshot recipient is not a participant.");
            }
        }
    }
}
