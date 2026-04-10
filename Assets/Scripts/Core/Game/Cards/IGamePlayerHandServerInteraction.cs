using System.Threading;
using System.Threading.Tasks;
using Core.Game.Dto.States.Cards;

namespace Core.Game.Cards
{
    public interface IGamePlayerHandServerInteraction
    {
        Task<PlayerHandStateData?> CreatePlayerHand(ulong playerId, CancellationToken ct);
    }
}