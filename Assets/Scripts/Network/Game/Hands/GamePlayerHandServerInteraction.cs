using System.Threading;
using System.Threading.Tasks;
using Core.Game.Cards;
using Core.Game.Dto.States.Cards;
using Network.Dto.Requests;
using Network.Requests;

namespace Network.Game.Hands
{
    public sealed class GamePlayerHandServerInteraction : IGamePlayerHandServerInteraction
    {
        private readonly INetworkService _networkService;

        public GamePlayerHandServerInteraction(INetworkService networkService)
        {
            _networkService = networkService;
        }

        public Task<PlayerHandStateData?> CreatePlayerHandAsync(ulong playerId, CancellationToken ct)
        {
            var request = new GameGetPlayerHandStateRequestDto
            {
                PlayerId = playerId,
            };

            return _networkService.GetDataAsync<GameGetPlayerHandStateRequestDto, PlayerHandStateData>(
                request,
                NetworkRequestType.GetPlayerHandState,
                ct);
        }

        public Task<bool> TrySkipCardAsync(CancellationToken ct)
        {
            var request = new GameSkipDestinyCardRequestDto();

            return _networkService.UpdateDataAsync(
                request, 
                NetworkRequestType.SkipDestinyCard, 
                ct);
        }
    }
}