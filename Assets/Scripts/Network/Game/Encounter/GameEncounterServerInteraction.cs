using System.Threading;
using System.Threading.Tasks;
using Core.Game.Encounter;
using Network.Dto.Requests;
using Network.Requests;

namespace Network.Game.Encounter
{
    public sealed class GameEncounterServerInteraction : IGameEncounterServerInteraction
    {
        private readonly INetworkService _networkService;
        
        public GameEncounterServerInteraction(INetworkService networkService)
        {
            _networkService = networkService;
        }
        
        public Task<bool> ChoosePlanetToAttackAsync(int planetId, CancellationToken ct)
        {
            var request = new GameChoosePlanetToAttackRequestDto()
            {
                PlanetId = planetId
            };

            return _networkService.UpdateDataAsync(
                request, 
                NetworkRequestType.ChoosePlanetToAttack,
                ct);
        }
    }
}