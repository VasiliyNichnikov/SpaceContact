using System.Collections.Generic;
using System.Linq;
using Core.Game.Mutation;
using Network.Dto;
using Network.Infrastructure;
using Unity.Netcode;
using VContainer;

namespace Network.Game.Mutation
{
    public sealed class GameEventRpcRelayNetwork : BaseNetworkSync
    {
        private INetworkSerializer _serializer = null!;
        private ClientEventsDispatcher _clientDispatcher = null!;
        private GameEventFactory _gameEventFactory = null!;

        [Inject]
        private void Constructor(
            INetworkSerializer serializer, 
            ClientEventsDispatcher clientDispatcher,
            GameEventFactory gameEventFactory)
        {
            _serializer = serializer;
            _clientDispatcher = clientDispatcher;
            _gameEventFactory = gameEventFactory;
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
            var gameEvents = gameEventsData
                .GameEvents
                .Select(e => _gameEventFactory.Create(e));
            _clientDispatcher.ApplyEvents(gameEvents);
        }
    }
}