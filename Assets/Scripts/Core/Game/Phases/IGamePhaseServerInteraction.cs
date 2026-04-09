using System.Threading;
using System.Threading.Tasks;
using Core.Game.Dto.States;

namespace Core.Game.Phases
{
    public interface IGamePhaseServerInteraction
    {
        Task<GalaxyStateData?> GetGalaxyStateAsync(CancellationToken ct);
    }
}