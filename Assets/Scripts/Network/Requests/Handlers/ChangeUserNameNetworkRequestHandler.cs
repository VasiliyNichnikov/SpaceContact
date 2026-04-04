using Core.User;
using Core.User.Dto.Requests;
using Logs;
using Network.Infrastructure;

namespace Network.Requests
{
    public sealed class ChangeUserNameNetworkRequestHandler : NetworkRequestHandler<ChangeUserNameRequestDto, EmptyResponseData>
    {
        private readonly ServerUsersRepository _serverUsersRepository;
        
        public ChangeUserNameNetworkRequestHandler(
            ServerUsersRepository serverUsersRepository,
            INetworkSerializer serializer) : base(serializer)
        {
            _serverUsersRepository = serverUsersRepository;
        }

        public override NetworkRequestType Type => 
            NetworkRequestType.PostChangeUserName;
        
        protected override EmptyResponseData? ProcessRequest(ChangeUserNameRequestDto request)
        {
            if (!_serverUsersRepository.ContainsUser(request.UserId))
            {
                Logger.Error($"ChangeUserNameNetworkRequestHandler.ProcessRequest: user with id {request.UserId} doesn't exist.");

                return null;
            }
            
            var user = _serverUsersRepository.Get(request.UserId);
            user.SetName(request.Name);
            
            return EmptyResponseData.Instance;
        }
    }
}