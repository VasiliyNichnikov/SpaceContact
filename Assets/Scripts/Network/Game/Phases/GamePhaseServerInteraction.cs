using System.Threading;
using System.Threading.Tasks;
using Core.Game.Dto.States;
using Core.Game.Phases;
using Network.Dto.Requests;
using Network.Requests;

namespace Network.Game.Phases
{
    public sealed class GamePhaseServerInteraction : IGamePhaseServerInteraction
    {
        private readonly INetworkService _networkService;

        public GamePhaseServerInteraction(INetworkService networkService)
        {
            _networkService = networkService;
        }
        
        public Task<GalaxyStateData?> GetGalaxyStateAsync(CancellationToken ct)
        {
            return _networkService.GetDataAsync<GameGalaxyStateRequestDto, GalaxyStateData>(
                new GameGalaxyStateRequestDto(),
                NetworkRequestType.GetGalaxyState,
                ct);
        }
    }
}