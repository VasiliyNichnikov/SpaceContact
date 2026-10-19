using System;
using Core.Game.Actions;
using Core.Game.Dto;
using Core.Game.Server;
using Logs;
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
            
            if (action is not ClientGameAction)
            {
                return GameApplyActionResponseDto.Rejected(action?.Id ?? Guid.Empty, "This action is not allowed from a client.");
            }

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
                        try
                        {
                            _eventBroadcaster.SendEvent(playerId, batchData);
                        }
                        catch (Exception exception)
                        {
                            Logger.Error($"Batch delivery to {playerId} failed: {exception}");
                        }
                    }
                }
            }

            return GameApplyActionResponseDto.Accepted(action.Id, senderBatch);
        }
    }
}
