using System.Threading;
using System.Threading.Tasks;
using Core.User;
using Core.User.Dto.Requests;
using Core.User.Dto.States;
using GeneralUtils;
using Network.Infrastructure;
using Network.Requests;
using Unity.Netcode;
using VContainer;
using Logger = Logs.Logger;

namespace Network.User
{
    public class UserNetworkState : NetworkBehaviour
    {
        private readonly NetworkVariable<ByteData> _playerState = new(
            new ByteData(),
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server);
        
        private ServerUsersRepository _serverRepository = null!;
        private ClientUsersRepository _clientRepository = null!;
        private ServerUserFactory _serverUserFactory = null!;
        private INetworkService _networkService = null!;
        private INetworkSerializer _serializer = null!;
        
        private IServerUser? _serverUser;

        [Inject]
        private void Construct(
            ServerUsersRepository serverRepository,
            ClientUsersRepository clientRepository,
            INetworkService networkService,
            ServerUserFactory serverUserFactory,
            INetworkSerializer serializer)
        {
            _serverRepository = serverRepository;
            _clientRepository = clientRepository;
            _networkService = networkService;
            _serverUserFactory = serverUserFactory;
            _serializer = serializer;
        }

        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                var isHost = OwnerClientId == NetworkManager.ServerClientId;
                var serverUser = _serverUserFactory.Create(
                    OwnerClientId, 
                    NetworkManager.LocalClientId, 
                    isHost);
                _serverRepository.Add(serverUser);
                _serverUser = serverUser;
                _serverUser.Changed += SendPlayerState;
                _clientRepository.Apply(NetworkManager.LocalClientId, _serverUser.ToState());
            }
            else
            {
                _playerState.OnValueChanged += OnStateReceived;
                LoadPlayerStateAsync().FireAndForget();
            }
        }

        public override void OnNetworkDespawn()
        {
            _clientRepository.Remove(OwnerClientId);
            
            if (!IsServer)
            {
                _playerState.OnValueChanged -= OnStateReceived;
                
                return;
            }

            if (_serverUser == null)
            {
                Logger.Error("PlayerNetworkState.OnNetworkDespawn: serverUser is null.");
                return;
            }
            
            _serverUser.Changed -= SendPlayerState;
            _serverRepository.Remove(_serverUser);
            _serverUser = null;
        }

        private void SendPlayerState()
        {
            if (_serverUser == null)
            {
                Logger.Error("PlayerNetworkState.SendPlayerState: serverUser is null.");
                
                return;
            }
            
            var state = _serverUser.ToState();
            var bytes = _serializer.Serialize(state);
            _playerState.Value = new ByteData(bytes);
            _clientRepository.Apply(NetworkManager.LocalClientId, _serverUser.ToState());
        }
        
        private void OnStateReceived(ByteData oldValue, ByteData newValue) => 
            ApplyPlayerState(newValue.Data);

        private void ApplyPlayerState(byte[]? bytes)
        {
            if (bytes == null || bytes.Length == 0)
            {
                return;
            }
            
            var state = _serializer.Deserialize<UserStateData>(bytes);
            _clientRepository.Apply(NetworkManager.LocalClientId, state);
        }

        private async Task LoadPlayerStateAsync()
        {
            // Костыль, но как будто лучше, чем усложнять код инициализации
            await TaskUtils.WaitUntil(() => _networkService.IsLoaded);
            
            var requestData = new UserStateRequestDto()
            {
                UserId = OwnerClientId
            };
            var state = await _networkService.GetDataAsync<UserStateRequestDto, UserStateData>(
                requestData,
                NetworkRequestType.GetUserState,
                CancellationToken.None);

            if (state == null)
            {
                Logger.Error("UserNetworkState.LoadPlayerStateAsync: couldn't upload state data.");

                return;
            }

            _clientRepository.Apply(NetworkManager.LocalClientId, state);
        }
    }
}