using System;
using System.Collections.Generic;
using System.Linq;
using Core.Game.Dto;
using Core.Game.Events;
using Logs;
using Network.Infrastructure;
using Unity.Netcode;
using VContainer;

namespace Network.Game
{
    public sealed class GameEventRpcRelayNetwork : BaseNetworkSync
    {
        private INetworkSerializer _serializer = null!;
        private GameEventBatchApplier _applier = null!;
        private GameSnapshotRecovery _recovery = null!;

        public override void OnNetworkDespawn()
        {
            _recovery.Dispose();
            base.OnNetworkDespawn();
        }

        public void SendEventsToClients(GameEventBatchData batchData, IReadOnlyList<ulong> targetClientIds)
        {
            if (!IsServer || !IsSpawned || targetClientIds.Count == 0)
            {
                return;
            }

            // The host uses the same serialized client projection and applier as remote clients.
            var data = new ByteData(_serializer.Serialize(batchData));
            ReceiveBattleEventRpc(data, RpcTarget.Group(targetClientIds.ToList(), RpcTargetUse.Temp));
        }

        protected override void OnNetworkSpawnInternal()
        {
            if (IsClient)
            {
                _recovery.Start();
            }
        }

        [Inject]
        private void Construct(INetworkSerializer serializer, GameEventBatchApplier applier, GameSnapshotRecovery recovery)
        {
            _serializer = serializer;
            _applier = applier;
            _recovery = recovery;
        }

        [Rpc(SendTo.SpecifiedInParams, InvokePermission = RpcInvokePermission.Server)]
        private void ReceiveBattleEventRpc(ByteData data, RpcParams rpcParams = default)
        {
            if (!IsClient || rpcParams.Receive.SenderClientId != NetworkManager.ServerClientId)
            {
                return;
            }

            GameEventBatchData? batch;
            try
            {
                batch = _serializer.Deserialize<GameEventBatchData>(data.Data);
            }
            catch (Exception exception)
            {
                Logger.Error($"Invalid game batch: {exception}");
                batch = null;
            }

            _applier.Apply(batch);
        }
    }
}
