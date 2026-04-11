using Core.Game.Dto.States;
using Core.Game.Galaxy.Server;
using Network.Dto.Requests;
using Network.Infrastructure;

namespace Network.Requests
{
    public class GetGalaxyStateNetworkRequestHandler : NetworkRequestHandler<GameGalaxyStateRequestDto, GalaxyStateData>
    {
        private readonly IGameServerGalaxyManager _serverGalaxyManager;
        
        public GetGalaxyStateNetworkRequestHandler(
            INetworkSerializer serializer, 
            IGameServerGalaxyManager serverGalaxyManager) : base(serializer)
        {
            _serverGalaxyManager = serverGalaxyManager;
        }

        public override NetworkRequestType Type => 
            NetworkRequestType.GetGalaxyState;
        
        protected override GalaxyStateData ProcessRequest(GameGalaxyStateRequestDto request, ulong senderId) => 
            _serverGalaxyManager.ToState();
    }
}