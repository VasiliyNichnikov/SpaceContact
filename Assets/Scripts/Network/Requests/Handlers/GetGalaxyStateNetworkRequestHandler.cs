using Core.Game;
using Core.Game.Dto.Requests;
using Core.Game.Dto.States;
using Network.Infrastructure;

namespace Network.Requests
{
    public class GetGalaxyStateNetworkRequestHandler : NetworkRequestHandler<GalaxyStateRequestDto, GalaxyStateData>
    {
        private readonly IGalaxyManagerNetwork _galaxyManagerNetwork;
        
        public GetGalaxyStateNetworkRequestHandler(
            INetworkSerializer serializer, 
            IGalaxyManagerNetwork galaxyManagerNetwork) : base(serializer)
        {
            _galaxyManagerNetwork = galaxyManagerNetwork;
        }

        public override NetworkRequestType Type => 
            NetworkRequestType.GetGalaxyState;
        
        protected override GalaxyStateData ProcessRequest(GalaxyStateRequestDto request) => 
            _galaxyManagerNetwork.GetState();
    }
}