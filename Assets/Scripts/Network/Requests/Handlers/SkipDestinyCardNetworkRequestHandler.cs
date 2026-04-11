using Core.Game.Phases.Server;
using Network.Dto.Requests;
using Network.Infrastructure;

namespace Network.Requests
{
    public class SkipDestinyCardNetworkRequestHandler : NetworkRequestHandler<GameSkipDestinyCardRequestDto, EmptyResponseData>
    {
        private readonly IGameServerDestinyPhaseResolver _destinyPhaseResolver;
        
        public SkipDestinyCardNetworkRequestHandler(
            INetworkSerializer serializer,
            IGameServerDestinyPhaseResolver destinyPhaseResolver) : base(serializer)
        {
            _destinyPhaseResolver = destinyPhaseResolver;
        }

        public override NetworkRequestType Type => 
            NetworkRequestType.SkipDestinyCard;
        
        protected override EmptyResponseData? ProcessRequest(GameSkipDestinyCardRequestDto request, ulong senderId)
        {
            var isCompleted = _destinyPhaseResolver.SkipDestiny(senderId);

            return isCompleted ? EmptyResponseData.Instance : null;
        }
    }
}