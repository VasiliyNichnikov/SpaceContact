using Core.Game;
using Core.Game.Dto;
using Network.Infrastructure;

namespace Network.Requests
{
    public sealed class GetGameSnapshotStateNetworkRequestHandler : NetworkRequestHandler<GameSnapshotRequestDto, GameSnapshotResponseDto>
    {
        private readonly GameStateProjector _projector;
        private readonly ServerGameContext _context;
        
        public GetGameSnapshotStateNetworkRequestHandler(
            INetworkSerializer serializer,
            GameStateProjector projector, ServerGameContext context) : base(serializer)
        {
            _projector = projector;
            _context = context;
        }

        public override NetworkRequestType Type => 
            NetworkRequestType.GetGameSnapshot;
        
        protected override GameSnapshotResponseDto? ProcessRequest(GameSnapshotRequestDto request, ulong senderId)
        {
            if (!_context.IsInitialized || !_context.Players.IsPlayerExist(senderId))
            {
                return null;
            }

            var snapshot = _projector.CreateSnapshot(senderId);
            var response = new GameSnapshotResponseDto
            {
                Snapshot = snapshot,
                Revision = _projector.Revision
            };
            
            return response;
        }
    }
}
