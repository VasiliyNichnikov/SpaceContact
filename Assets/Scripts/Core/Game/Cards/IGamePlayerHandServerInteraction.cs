using System.Threading;
using System.Threading.Tasks;
using Core.Game.Dto.States.Cards;

namespace Core.Game.Cards
{
    public interface IGamePlayerHandServerInteraction
    {
        Task<PlayerHandStateData?> CreatePlayerHandAsync(ulong playerId, CancellationToken ct);
        
        Task<bool> TrySkipCardAsync(CancellationToken ct);
    }
}