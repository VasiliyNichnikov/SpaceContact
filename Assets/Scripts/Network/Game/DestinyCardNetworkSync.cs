using Core.Game.Cards;
using Core.Game.Dto.States.Cards;
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
        /// Other
        /// </summary>
        private IGameClientDestinyCardController _clientDestinyCardController = null!;
        private INetworkSerializer _serializer = null!;
        private IObjectResolver _resolver = null!;
        
        public IPrefabInitializerOnClients Initializer => _initializer;
        
        [Inject]
        private void Constructor(
            INetworkSerializer serializer, 
            IObjectResolver resolver,
            IGameClientDestinyCardController clientDestinyCardController)
        {
            _serializer = serializer;
            _resolver = resolver;
            _clientDestinyCardController = clientDestinyCardController;
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
            // Данный стейт должен быть одинаков и на клиентах и на сервере
            ApplyDestinyCardState(bytes);
        }

        private void OnStateReceived(ByteData oldValue, ByteData newValue) => 
            ApplyDestinyCardState(newValue.Data);

        private void ApplyDestinyCardState(byte[]? bytes)
        {
            if (bytes == null || bytes.Length == 0)
            {
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