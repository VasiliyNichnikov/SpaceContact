using Core.Game.Cards;
using Core.Game.Dto.States.Cards;
using Core.Game.Players;
using Logs;
using Network.Dto.Requests;
using Network.Infrastructure;

namespace Network.Requests
{
    public sealed class CollectPlayerHandStateNetworkRequestHandler : NetworkRequestHandler<GamePlayerHandStateRequestDto, PlayerHandStateData>
    {
        private readonly GamePlayersRegistry _registry;
        private readonly IGameServerCardsManager _serverCardsManager;
        
        public CollectPlayerHandStateNetworkRequestHandler(
            INetworkSerializer serializer,
            IGameServerCardsManager serverCardsManager,
            GamePlayersRegistry registry) : base(serializer)
        {
            _registry = registry;
            _serverCardsManager = serverCardsManager;
        }

        public override NetworkRequestType Type => 
            NetworkRequestType.CollectPlayerHandState;

        protected override PlayerHandStateData? ProcessRequest(GamePlayerHandStateRequestDto request, ulong senderId)
        {
            var player = _registry.GetPlayerById(request.PlayerId);

            if (player is not ServerGamePlayer serverPlayer)
            {
                Logger.Error($"{nameof(CollectPlayerHandStateNetworkRequestHandler)}.{nameof(ProcessRequest)}: player not supported.");
                
                return null;
            }

            if (serverPlayer.HandState == null)
            {
                var createdHand = _serverCardsManager.CreatePlayerHand();
                serverPlayer.UpdateHandState(createdHand);
            }
            
            var originalState = serverPlayer.HandState!;
            
            if (request.PlayerId == senderId)
            {
                return originalState;
            }
            
            var copyState = new PlayerHandStateData
            {
                NumberOfCards = originalState.NumberOfCards,
                SpaceCardsOnYourHand = null
            };
            
            return copyState;
        }
    }
}