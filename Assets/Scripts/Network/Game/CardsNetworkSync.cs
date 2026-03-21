using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Game.Dto.Requests;
using Core.Game.Dto.States.Cards;
using Core.Game.Players;
using Core.Game.Players.Visitors;
using Logs;
using Network.Requests;
using Network.Utils;
using Unity.Netcode;
using VContainer;

namespace Network.Game
{
    public class CardsNetworkSync : NetworkBehaviour
    {
        private IGamePlayer _player = null!;
        private GamePlayersRegistry _registry = null!;

        /// <summary>
        /// Server Only
        /// </summary>
        private INetworkService _networkService = null!;

        [Inject]
        private void Constructor(
            INetworkService networkService,
            GamePlayersRegistry registry)
        {
            _networkService = networkService;
            _registry = registry;
        }

        public override void OnNetworkSpawn()
        {
            _player = _registry.GetPlayerById(OwnerClientId);

            if (IsServer)
            {
                return;
            }
            
            if (IsOwner)
            {
                LoadStartingHandStateAsync().FireAndForget();
            }
            else
            {
                LoadStartingCardsCountAsync().FireAndForget();
            }
        }

        private Task LoadStartingHandStateAsync() => 
            LoadStartingCardsAsync(false, CancellationToken.None);

        private Task LoadStartingCardsCountAsync() =>
            LoadStartingCardsAsync(true, CancellationToken.None);

        private async Task LoadStartingCardsAsync(bool onlyNumberCardsInHand, CancellationToken ct)
        {
            var state = await GetPlayerHandStateDataAsync(onlyNumberCardsInHand, ct);

            if (state == null)
            {
                return;
            }
            
            var handDistributionVisitor = new GamePlayerHandDistributionVisitor(state);
            _player.Apply(handDistributionVisitor);
        }
        
        private async Task<PlayerHandStateData?> GetPlayerHandStateDataAsync(bool onlyNumberCardsInHand, CancellationToken ct)
        {
            var request = new PlayerHandStateRequestDto
            {
                OnlyNumberCardsInHand = onlyNumberCardsInHand,
                PlayerId = OwnerClientId,
            };
            
            var state = await _networkService.GetDataAsync<PlayerHandStateRequestDto, PlayerHandStateData>(
                request,
                NetworkRequestType.GetPlayerHandState,
                ct);

            if (state == null || ct.IsCancellationRequested)
            {
                Logger.Error("CardsNetworkSync.GetPlayerHandStateDataAsync: couldn't load player hand state data.");
                
                return null;
            }
            
            return state;
        }
    }
}