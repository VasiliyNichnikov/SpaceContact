using System;
using Core.Game.Dto;
using Core.Game.Server;

namespace Network.Game
{
    public sealed class GameServerEventBroadcaster : IServerEventBroadcaster
    {
        private GameEventRpcRelayNetwork? _relay;

        public void SetRelayObject(GameEventRpcRelayNetwork relay)
        {
            _relay = relay;
        }

        public void SendEvent(ulong recipientPlayerId, GameEventBatchData batchData)
        {
            var relay = _relay;
            
            if (relay == null || !relay.IsSpawned || !relay.IsServer)
            {
                throw new InvalidOperationException("The server event relay is not ready.");
            }

            relay.SendEventsToClients(batchData, new[] { recipientPlayerId });
        }
    }
}
