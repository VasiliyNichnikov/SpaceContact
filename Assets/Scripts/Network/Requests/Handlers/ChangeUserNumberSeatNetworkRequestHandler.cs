using Core.User;
using Core.User.Dto.Requests;
using Logs;
using Network.Infrastructure;

namespace Network.Requests
{
    public sealed class ChangeUserNumberSeatNetworkRequestHandler : NetworkRequestHandler<ChangeUserSeatNumberRequestDto, EmptyResponseData>
    {
        private readonly ServerUsersRepository _serverUsersRepository; 
        
        public ChangeUserNumberSeatNetworkRequestHandler(
            ServerUsersRepository serverUsersRepository,
            INetworkSerializer serializer) : base(serializer)
        {
            _serverUsersRepository = serverUsersRepository;
        }

        public override NetworkRequestType Type =>
            NetworkRequestType.PostChangeUserSeatNumber;
        
        protected override EmptyResponseData? ProcessRequest(ChangeUserSeatNumberRequestDto request)
        {
            if (!_serverUsersRepository.ContainsUser(request.UserId))
            {
                Logger.Error($"ChangeUserNumberSeatNetworkRequestHandler.ProcessRequest: user with id {request.UserId} doesn't exist.");

                return null;
            }
            
            var user = _serverUsersRepository.Get(request.UserId);
            user.SetSeatNumber(request.SeatNumber);
            
            return EmptyResponseData.Instance;
        }
    }
}