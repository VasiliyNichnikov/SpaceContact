using Core.Game.Encounter;
using Network.Dto.Requests;
using Network.Infrastructure;

namespace Network.Requests
{
    public sealed class ChoosePlanetToAttackNetworkRequestHandler : NetworkRequestHandler<GameChoosePlanetToAttackRequestDto, EmptyResponseData>
    {
        private readonly IGameServerEncounterManager _serverEncounterManager;
        
        public ChoosePlanetToAttackNetworkRequestHandler(
            INetworkSerializer serializer,
            IGameServerEncounterManager serverEncounterManager) : base(serializer)
        {
            _serverEncounterManager = serverEncounterManager;
        }

        public override NetworkRequestType Type => 
            NetworkRequestType.ChoosePlanetToAttack;
        
        protected override EmptyResponseData? ProcessRequest(GameChoosePlanetToAttackRequestDto request, ulong senderId)
        {
            var isCompleted = _serverEncounterManager.SetPlanetToAttack(senderId, request.PlanetId);
            
            return isCompleted ? EmptyResponseData.Instance : null;
        }
    }
}