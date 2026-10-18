using System.Threading;
using System.Threading.Tasks;
using Core.Game.Dto;

namespace Core.Game.Actions
{
    public interface IGameActionServerInteraction
    {
        // Cancellation stops waiting locally; it does not roll back an action already accepted by the server.
        // Implementations must honor ct and preserve the request ID in SourceActionId.
        // On cancellation, return null with a normally completed Task; never throw or return a canceled Task.
        // Callers check ct.IsCancellationRequested after awaiting the response.
        Task<GameApplyActionResponseDto?> SendAsync(IGameAction action, GameActor actor, CancellationToken ct);
    }
}