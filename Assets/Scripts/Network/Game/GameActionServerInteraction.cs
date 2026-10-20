using System.Threading;
using System.Threading.Tasks;
using Core.Game.Actions;
using Core.Game.Dto;
using Network.Requests;

namespace Network.Game
{
    public sealed class GameActionServerInteraction : IGameActionServerInteraction
    {
        private readonly INetworkService _network;

        public GameActionServerInteraction(INetworkService network)
        {
            _network = network;
        }

        public async Task<GameApplyActionResponseDto?> SendAsync(ClientGameAction action, GameActor actor, CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
            {
                return null;
            }

            if (actor.IsSystem || !actor.PlayerId.HasValue)
            {
                return GameApplyActionResponseDto.Rejected(action.Id, "Only player actions can be sent over the network.");
            }

            var response = await _network.GetDataAsync<GameApplyActionRequestDto, GameApplyActionResponseDto>(
                new GameApplyActionRequestDto { Action = action }, NetworkRequestType.ApplyGameAction, ct);
            return ct.IsCancellationRequested ? null : response;
        }
    }
}
