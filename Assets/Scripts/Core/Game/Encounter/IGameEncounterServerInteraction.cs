using System.Threading;
using System.Threading.Tasks;

namespace Core.Game.Encounter
{
    public interface IGameEncounterServerInteraction
    {
        Task<bool> ChoosePlanetToAttackAsync(int planetId, CancellationToken ct);
    }
}