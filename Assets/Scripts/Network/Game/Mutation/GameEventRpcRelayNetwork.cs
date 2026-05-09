using System.Collections.Generic;
using System.Linq;
using Core.Game.Mutation;
using Logs;
using Network.Dto;
using Network.Infrastructure;
using Unity.Netcode;
using VContainer;

namespace Network.Game.Mutation
{
    public sealed class GameEventRpcRelayNetwork : BaseNetworkSync
    {
        private INetworkSerializer _serializer = null!;
        private GameClientEventContext _clientEventContext = null!;

        [Inject]
        private void Constructor(
            INetworkSerializer serializer, 
            GameClientEventContext clientEventContext)
        {
            _serializer = serializer;
            _clientEventContext = clientEventContext;
        }
        
        protected override void OnNetworkSpawnInternal()
        {
            // nothing
        }
        
        public void SendEventsToClients(GameEventsToClientsData gameEventsData, IReadOnlyList<ulong> targetClientIds)
        {
            if (!IsServer)
            {
                return;
            }

            var targetClients = targetClientIds.ToList();
            const ulong serverClientId = NetworkManager.ServerClientId;

            if (targetClients.Contains(serverClientId))
            {
                targetClients.Remove(serverClientId);
                ApplyEvents(gameEventsData);
            }

            if (targetClients.Count == 0)
            {
                return;
            }
            
            var rpcParams = RpcTarget.Group(targetClients, RpcTargetUse.Temp);
            var bytes = _serializer.Serialize(gameEventsData);
            var data = new ByteData(bytes);
            ReceiveBattleEventRpc(data, rpcParams);
        }
        
        [Rpc(SendTo.SpecifiedInParams)]
        private void ReceiveBattleEventRpc(ByteData data, RpcParams _ = default)
        {
            var gameEventsData = _serializer.Deserialize<GameEventsToClientsData>(data.Data);
            ApplyEvents(gameEventsData);
        }

        private void ApplyEvents(GameEventsToClientsData gameEventsData)
        {
            if (gameEventsData.GameEvents.Length == 0)
            {
                Logger.Error($"{nameof(GameEventRpcRelayNetwork)}.{nameof(ApplyEvents)}: gameEvents is empty.");
            }
            
            foreach (var gameEvent in gameEventsData.GameEvents)
            {
                gameEvent.Apply(_clientEventContext);
            }
        }
    }
}