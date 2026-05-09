using Core.Game.Phases.Server;
using Network.Dto.Requests;
using Network.Infrastructure;

namespace Network.Requests
{
    public class ChangePlayerReadinessRequestHandler : NetworkRequestHandler<GameChangePlayerReadinessRequestDto, EmptyResponseData>
    {
        private readonly GameServerChangerStatePlayersReadiness _readyMadePlayersCollection;
        
        public ChangePlayerReadinessRequestHandler(
            GameServerChangerStatePlayersReadiness readyMadePlayersCollection,
            INetworkSerializer serializer) : base(serializer)
        {
            _readyMadePlayersCollection = readyMadePlayersCollection;
        }

        public override NetworkRequestType Type => 
            NetworkRequestType.ChangePlayerReadiness;
        
        protected override EmptyResponseData? ProcessRequest(GameChangePlayerReadinessRequestDto request, ulong senderId)
        {
            var isCompleted = _readyMadePlayersCollection.ChangePlayerReadiness(senderId, request.IsReady);

            return isCompleted 
                ? EmptyResponseData.Instance 
                : null;
        }
    }
}