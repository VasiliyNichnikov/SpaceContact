using System.Threading;
using System.Threading.Tasks;

namespace Core.User
{
    public interface IUserServerInteraction
    {
        Task ChangeMyNameAsync(string name, CancellationToken ct = default);

        Task ChangeMyColorAsync(int colorId, CancellationToken ct = default);

        Task ChangeMySeatNumberAsync(int seatNumber, CancellationToken ct = default);
    }
}