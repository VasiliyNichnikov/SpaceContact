using System.Threading;
using System.Threading.Tasks;
using Core.Game.Dto;

namespace Core.Game.Actions
{
    public interface IGameActionServerInteraction
    {
        Task<GameApplyActionResponseDto?> SendAsync(IGameAction action, GameActor actor, CancellationToken ct);
    }
}