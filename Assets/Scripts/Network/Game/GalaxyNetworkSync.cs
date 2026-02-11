using Core.Game;
using Core.Game.Dto.States;
using Network.Infrastructure;
using Unity.Netcode;
using VContainer;

namespace Network.Game
{
    public class GalaxyNetworkSync : NetworkBehaviour
    {
        private readonly NetworkVariable<ByteData> _galaxyState = new (
            new ByteData(), 
            NetworkVariableReadPermission.Everyone, 
            NetworkVariableWritePermission.Server);

        private IGalaxyManagerNetwork _netGalaxy = null!;
        private INetworkSerializer _serializer = null!;
        
        [Inject]
        private void Construct(
            IGalaxyManagerNetwork galaxyManager,
            INetworkSerializer serializer)
        {
            _netGalaxy = galaxyManager;
            _serializer = serializer;
        }

        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                _netGalaxy.OnStateChanged += SendGalaxyState; 
            }
            else
            {
                _galaxyState.OnValueChanged += OnStateReceived;

                if (!_galaxyState.Value.IsEmpty)
                {
                    ApplyGalaxyState(_galaxyState.Value.Data);
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
    }
}