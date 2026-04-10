using System.Threading;
using System.Threading.Tasks;

namespace Core.Game.Cards
{
    public interface IGameClientPlayerCardsDeckService
    {
        Task InitPlayersHands(CancellationToken ct);
    }
}