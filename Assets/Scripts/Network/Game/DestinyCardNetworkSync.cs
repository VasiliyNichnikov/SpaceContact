using Core.Game.Cards;
using Core.Game.Dto.States.Cards;
using Logs;
using Network.Infrastructure;
using Unity.Netcode;
using VContainer;

namespace Network.Game
{
    public class DestinyCardNetworkSync : NetworkBehaviour
    {
        private readonly NetworkVariable<ByteData> _destinyCardState = new(
            new ByteData(),
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server);
        
        /// <summary>
        /// Only Server
        /// </summary>
        private readonly PrefabInitializerOnClients _initializer = new();
        private IGameServerDestinyCardController? _serverDestinyCardController;
        
        /// <summary>
        /// Client Only
        /// </summary>
        private IGameClientDestinyCardController? _clientDestinyCardController;
        
        /// <summary>
        /// Other
        /// </summary>
        private INetworkSerializer _serializer = null!;
        private IObjectResolver _resolver = null!;
        
        public IPrefabInitializerOnClients Initializer => _initializer;
        
        [Inject]
        private void Constructor(
            INetworkSerializer serializer, 
            IObjectResolver resolver)
        {
            _serializer = serializer;
            _resolver = resolver;
        }

        public override void OnNetworkSpawn()
        {
            _initializer.SetPrefabId(NetworkObjectId);

            if (IsServer)
            {
                _serverDestinyCardController = _resolver.Resolve<IGameServerDestinyCardController>();
                _serverDestinyCardController.CardChanged += SendCardState;
            }
            else
            {
                _clientDestinyCardController = _resolver.Resolve<IGameClientDestinyCardController>();
                _destinyCardState.OnValueChanged += OnStateReceived;
            }

            ReportLoadedServerRpc();
        }

        public override void OnNetworkDespawn()
        {
            if (IsServer)
            {
                if (_serverDestinyCardController != null)
                {
                    _serverDestinyCardController.CardChanged -= SendCardState;
                }
            }
            else
            {
                _destinyCardState.OnValueChanged -= OnStateReceived;
            }
        }

        private void SendCardState()
        {
            var state = _serverDestinyCardController!.GetState();
            var bytes = _serializer.Serialize(state);
            _destinyCardState.Value = new ByteData(bytes);
        }

        private void OnStateReceived(ByteData oldValue, ByteData newValue) => 
            ApplyDestinyCardState(newValue.Data);

        private void ApplyDestinyCardState(byte[]? bytes)
        {
            if (bytes == null || bytes.Length == 0)
            {
                return;
            }

            if (_clientDestinyCardController == null)
            {
                Logger.Error("DestinyCardNetworkSync.ApplyDestinyCardState: clientDestinyCardController is null.");
                return;
            }
            
            var state = _serializer.Deserialize<DestinyCardStateData>(bytes);
            _clientDestinyCardController.UpdateState(state);
        }
        
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