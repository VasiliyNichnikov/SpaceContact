using Core.Game.Dto.States.Cards;
using Core.Game.Players;
using Logs;
using Network.Dto.Requests;
using Network.Infrastructure;

namespace Network.Requests
{
    public sealed class GetPlayerHandStateNetworkRequestHandler : NetworkRequestHandler<GameGetPlayerHandStateRequestDto, PlayerHandStateData>
    {
        private readonly GamePlayersRegistry _registry;
        
        public GetPlayerHandStateNetworkRequestHandler(
            INetworkSerializer serializer,
            GamePlayersRegistry registry) : base(serializer)
        {
            _registry = registry;
        }

        public override NetworkRequestType Type => 
            NetworkRequestType.GetPlayerHandState;

        protected override PlayerHandStateData? ProcessRequest(GameGetPlayerHandStateRequestDto request, ulong senderId)
        {
            var player = _registry.GetPlayerById(request.PlayerId);

            if (player is not ServerGamePlayer serverPlayer)
            {
                Logger.Error($"{nameof(GetPlayerHandStateNetworkRequestHandler)}.{nameof(ProcessRequest)}: player not supported.");
                
                return null;
            }
            
            var handState = serverPlayer.HandState;

            if (handState == null)
            {
                Logger.Error($"{nameof(GetPlayerHandStateNetworkRequestHandler)}.{nameof(ProcessRequest)}: handState for player {request.PlayerId} not found.");
                
                return null;
            }
            
            
            if (request.PlayerId == senderId)
            {
                return handState;
            }
            
            var copyState = new PlayerHandStateData
            {
                NumberOfCards = handState.NumberOfCards,
                SpaceCardsOnYourHand = null
            };
            
            return copyState;
        }
    }
}