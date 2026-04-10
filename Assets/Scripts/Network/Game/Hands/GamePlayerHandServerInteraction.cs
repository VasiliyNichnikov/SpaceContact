using System.Threading;
using System.Threading.Tasks;
using Core.Game.Cards;
using Core.Game.Dto.Requests;
using Core.Game.Dto.States.Cards;
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

        public Task<PlayerHandStateData?> CreatePlayerHand(ulong playerId, CancellationToken ct)
        {
            var request = new PlayerHandStateRequestDto
            {
                PlayerId = playerId,
            };

            return _networkService.GetDataAsync<PlayerHandStateRequestDto, PlayerHandStateData>(
                request,
                NetworkRequestType.CollectPlayerHandState,
                ct);
        }
    }
}