using Core.User;
using Core.User.Dto.Requests;
using Core.User.Dto.States;
using Logs;
using Network.Infrastructure;

namespace Network.Requests
{
    public sealed class GetUserStateNetworkRequestHandler : NetworkRequestHandler<UserStateRequestDto, UserStateData>
    {
        private readonly ServerUsersRepository _serverUsersRepository;
        
        public GetUserStateNetworkRequestHandler(
            ServerUsersRepository serverUsersRepository, 
            INetworkSerializer serializer) : base(serializer)
        {
            _serverUsersRepository = serverUsersRepository;
        }

        public override NetworkRequestType Type => 
            NetworkRequestType.GetUserState;
        
        protected override UserStateData? ProcessRequest(UserStateRequestDto request)
        {
            if (!_serverUsersRepository.ContainsUser(request.UserId))
            {
                Logger.Error($"GetUserStateNetworkRequestHandler.ProcessRequest: user with id {request.UserId} doesn't exist.");
                
                return null;
            }
            
            var user = _serverUsersRepository.Get(request.UserId);
            
            return user.ToState();
        }
    }
}