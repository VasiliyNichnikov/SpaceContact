using Core.Game.Actions;
using Core.Game.Dto;
using Core.Game.Server;
using Network.Infrastructure;

namespace Network.Requests
{
    public sealed class ApplyGameActionNetworkRequestHandler : NetworkRequestHandler<GameApplyActionRequestDto, GameApplyActionResponseDto>
    {
        private readonly GameActionExecutor _executor;
        private readonly IServerEventBroadcaster _eventBroadcaster;

        public ApplyGameActionNetworkRequestHandler(
            INetworkSerializer serializer,
            GameActionExecutor executor,
            IServerEventBroadcaster eventBroadcaster) : base(serializer)
        {
            _executor = executor;
            _eventBroadcaster = eventBroadcaster;
        }

        public override NetworkRequestType Type => NetworkRequestType.ApplyGameAction;

        protected override GameApplyActionResponseDto ProcessRequest(GameApplyActionRequestDto request, ulong senderId)
        {
            var action = request.Action;
            var result = _executor.Execute(action, GameActor.Player(senderId));

            if (!result.IsSuccess)
            {
                return GameApplyActionResponseDto.Rejected(action.Id, result.ErrorMessage);
            }

            var senderBatch = result.BatchesByRecipient[senderId];

            if (!result.IsDuplicate && result.HasChanges)
            {
                foreach (var (playerId, batchData) in result.BatchesByRecipient)
                {
                    if (senderId != playerId)
                    {
                        _eventBroadcaster.SendEvent(playerId, batchData);
                    }
                }
            }

            return GameApplyActionResponseDto.Accepted(action.Id, senderBatch);
        }
    }
}
