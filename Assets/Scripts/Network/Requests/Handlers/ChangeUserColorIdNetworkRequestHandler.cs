using Core.User;
using Logs;
using Network.Dto.Requests;
using Network.Infrastructure;

namespace Network.Requests
{
    public sealed class ChangeUserColorIdNetworkRequestHandler : NetworkRequestHandler<ChangeUserColorRequestDto, EmptyResponseData>
    {
        private readonly ServerUsersRepository _serverUsersRepository;
        
        public ChangeUserColorIdNetworkRequestHandler(
            ServerUsersRepository serverUsersRepository,
            INetworkSerializer serializer) : base(serializer)
        {
            _serverUsersRepository = serverUsersRepository;
        }

        public override NetworkRequestType Type => 
            NetworkRequestType.PostChangeUserColorId;
        
        protected override EmptyResponseData? ProcessRequest(ChangeUserColorRequestDto request, ulong senderId)
        {
            if (!_serverUsersRepository.ContainsUser(request.UserId))
            {
                Logger.Error($"ChangeUserColorIdNetworkRequestHandler.ProcessRequest: user with id {request.UserId} doesn't exist.");

                return null;
            }
            
            var user = _serverUsersRepository.Get(request.UserId);
            user.SetColorId(request.SelectedColorId);
            
            return EmptyResponseData.Instance;
        }
    }
}