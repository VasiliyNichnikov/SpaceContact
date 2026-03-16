using System.Threading;
using System.Threading.Tasks;
using Core.Game;
using Core.Game.Dto.Requests;
using Core.Game.Dto.States;
using Logs;
using Network.Infrastructure;
using Network.Requests;
using Network.Utils;
using Unity.Netcode;
using VContainer;

namespace Network.Game
{
    public class GalaxyNetworkSync : NetworkBehaviour
    {
        private readonly NetworkVariable<ByteData> _galaxyState = new(
            new ByteData(),
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server);

        /// <summary>
        /// Only Server
        /// </summary>
        private readonly PrefabInitializerOnClients _initializer = new();

        private IGalaxyManagerNetwork _netGalaxy = null!;

        private INetworkSerializer _serializer = null!;
        private INetworkService _networkService = null!;

        public IPrefabInitializerOnClients Initializer => _initializer;

        [Inject]
        private void Construct(
            IGalaxyManagerNetwork galaxyManager,
            INetworkSerializer serializer,
            INetworkService networkService)
        {
            _netGalaxy = galaxyManager;
            _serializer = serializer;
            _networkService = networkService;
        }

        public override void OnNetworkSpawn()
        {
            _initializer.SetPrefabId(NetworkObjectId);

            if (IsServer)
            {
                _netGalaxy.OnStateChanged += SendGalaxyState;
                _netGalaxy.ServerGalaxyLoaded();
                ReportLoadedServerRpc();
            }
            else
            {
                _galaxyState.OnValueChanged += OnStateReceived;

                if (!_galaxyState.Value.IsEmpty)
                {
                    ApplyGalaxyState(_galaxyState.Value.Data);
                }
                else
                {
                    LoadGalaxyStartingStateAsync().FireAndForget();
                }
            }
        }

        public override void OnNetworkDespawn()
        {
            if (IsServer)
            {
                _netGalaxy.OnStateChanged -= SendGalaxyState;
            }
            else
            {
                _galaxyState.OnValueChanged -= OnStateReceived;
            }
        }

        private void SendGalaxyState()
        {
            var state = _netGalaxy.GetState();
            var bytes = _serializer.Serialize(state);
            _galaxyState.Value = new ByteData(bytes);
        }

        private void OnStateReceived(ByteData oldValue, ByteData newValue)
        {
            ApplyGalaxyState(newValue.Data);
        }

        private void ApplyGalaxyState(byte[]? bytes)
        {
            if (bytes == null || bytes.Length == 0)
            {
                return;
            }

            var state = _serializer.Deserialize<GalaxyStateData>(bytes);
            _netGalaxy.ApplyStateData(state);
        }

        private async Task LoadGalaxyStartingStateAsync()
        {
            var state = await _networkService.GetDataAsync<GalaxyStateRequestDto, GalaxyStateData>(
                new GalaxyStateRequestDto(),
                NetworkRequestType.GetGalaxyState,
                CancellationToken.None);

            if (state == null)
            {
                Logger.Error("GalaxyNetworkSync.LoadGalaxyStartingStateAsync: couldn't upload state data.");

                return;
            }

            _netGalaxy.ApplyStateData(state);
            ReportLoadedServerRpc();
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