using Core.Game.Dto.Requests;
using Core.Game.Dto.States.Cards;
using Core.Game.Players;
using Logs;
using Network.Infrastructure;

namespace Network.Requests
{
    public class GetPlayerHandStateNetworkRequestHandler : NetworkRequestHandler<PlayerHandStateRequestDto, PlayerHandStateData>
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

        protected override PlayerHandStateData? ProcessRequest(PlayerHandStateRequestDto request)
        {
            var player = _registry.GetPlayerById(request.PlayerId);

            if (player is ServerGamePlayer { HandState: not null } serverPlayer)
            {
                var originalState = serverPlayer.HandState;
                var copyState = new PlayerHandStateData
                {
                    NumberOfCards = originalState.NumberOfCards,
                    SpaceCardsOnYourHand = originalState.SpaceCardsOnYourHand,
                };

                if (request.OnlyNumberCardsInHand)
                {
                    copyState.SpaceCardsOnYourHand = null;
                }

                return copyState;
            }
            
            Logger.Error("GetPlayerHandStateNetworkRequestHandler.ProcessRequest: failed to get the player's hand.");
            
            return null;
        }
    }
}