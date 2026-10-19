using Unity.Netcode;

namespace Network.Game
{
    public class GameRevisionAckNetwork : NetworkBehaviour
    {
        private ServerClientRevisionTracker? _tracker;

        public void SetServerTracker(ServerClientRevisionTracker tracker)
        {
            _tracker = tracker;
        }

        public void Send(long revision, long phaseRevision)
        {
            if (IsClient && IsSpawned)
            {
                AcknowledgeRevisionRpc(revision, phaseRevision);
            }
        }

        [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
        private void AcknowledgeRevisionRpc(long revision, long phaseRevision, RpcParams rpcParams = default)
        {
            _tracker?.Acknowledge(rpcParams.Receive.SenderClientId, revision, phaseRevision);
        }
    }
}
