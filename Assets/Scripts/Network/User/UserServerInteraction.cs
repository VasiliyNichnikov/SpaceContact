using System.Threading;
using System.Threading.Tasks;
using Core.User;
using Core.User.Dto.Requests;
using Logs;
using Network.Requests;

namespace Network.User
{
    public sealed class UserServerInteraction : IUserServerInteraction
    {
        private readonly INetworkService _networkService;
        private readonly ClientUsersRepository _usersRepository;
        private readonly IUsersColorProvider _usersColorProvider;
        private readonly IUsersSeatsProvider _usersSeatsProvider;
        
        public UserServerInteraction(
            INetworkService networkService, 
            ClientUsersRepository usersRepository,
            IUsersColorProvider usersColorProvider,
            IUsersSeatsProvider usersSeatsProvider)
        {
            _networkService = networkService;
            _usersRepository = usersRepository;
            _usersColorProvider = usersColorProvider;
            _usersSeatsProvider = usersSeatsProvider;
        }

        private ulong CurrentUserId => 
            _usersRepository.GetCurrentUser().ClientId;
        
        public async Task ChangeMyNameAsync(string name, CancellationToken ct = default)
        {
            var requestData = new ChangeUserNameRequestDto
            {
                UserId = CurrentUserId,
                Name = name
            };
            
            var isSuccess = await _networkService.UpdateDataAsync(
                requestData,
                NetworkRequestType.PostChangeUserName, 
                ct);

            if (!isSuccess)
            {
                Logger.Error("UserServerInteraction.ChangeMyNameAsync: failed to update user name.");
            }
        }
        
        public async Task ChangeMyColorAsync(int colorId, CancellationToken ct = default)
        {
            if (!_usersColorProvider.IsColorAvailableForSelection(colorId))
            {
                Logger.Error("UserServerInteraction.ChangeMyColorAsync: the color is occupied by another user.");
                return;
            }
            
            var requestData = new ChangeUserColorRequestDto
            {
                UserId = CurrentUserId,
                SelectedColorId = colorId
            };
            
            var isSuccess = await _networkService.UpdateDataAsync(
                requestData,
                NetworkRequestType.PostChangeUserColorId, 
                ct);

            if (!isSuccess)
            {
                Logger.Error("UserServerInteraction.ChangeMyColorAsync: failed to update user color.");
            }
        }

        public async Task ChangeMySeatNumberAsync(int seatNumber, CancellationToken ct = default)
        {
            if (!_usersSeatsProvider.IsSeatAvailable(seatNumber))
            {
                Logger.Error("UserServerInteraction.ChangeMySeatNumberAsync: the seat number is occupied by another user.");
                return;
            }
            
            var requestData = new ChangeUserSeatNumberRequestDto
            {
                UserId = CurrentUserId,
                SeatNumber = seatNumber
            };
            
            var isSuccess = await _networkService.UpdateDataAsync(
                requestData,
                NetworkRequestType.PostChangeUserSeatNumber, 
                ct);
            
            if (!isSuccess)
            {
                Logger.Error("UserServerInteraction.ChangeMyColorAsync: failed to update user seat number.");
            }
        }
    }
}