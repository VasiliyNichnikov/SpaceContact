using Unity.Netcode;

namespace Network
{
    public abstract class BaseNetworkSync : NetworkBehaviour
    {
        private readonly PrefabInitializerOnClients _initializer = new();
        
        public IPrefabInitializerOnClients Initializer => 
            _initializer;

        public override void OnNetworkSpawn()
        {
            _initializer.SetPrefabId(NetworkObjectId);
            OnNetworkSpawnInternal();

            if (IsClient)
            {
                ReportLoadedServerRpc();
            }
        }

        protected abstract void OnNetworkSpawnInternal();
        
        [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
        private void ReportLoadedServerRpc(RpcParams rpcParams = default)
        {
            if (!IsServer)
            {
                return;
            }

            _initializer.LoadOnClient(rpcParams.Receive.SenderClientId);
        }
    }
}