using Core.Game.Players;
using Unity.Netcode;
using VContainer;

namespace Network.Game
{
    public class GamePlayerNetworkSync : NetworkBehaviour
    {
        /// <summary>
        /// Only Server
        /// </summary>
        private readonly PrefabInitializerOnClients _initializer = new();
        
        public IPrefabInitializerOnClients Initializer => _initializer;

        public override void OnNetworkSpawn()
        {
            _initializer.SetPrefabId(NetworkObjectId);

            if (IsClient)
            {
                ReportLoadedServerRpc();
            }
        }

        [Rpc(SendTo.Server, InvokePermission=RpcInvokePermission.Everyone)]
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