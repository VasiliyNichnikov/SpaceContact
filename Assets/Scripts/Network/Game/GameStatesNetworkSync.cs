using Core.Game.Dto.States;
using Core.Game.Dto.States.Cards;
using Core.Game.Encounter;
using Core.Game.Phases.Client;
using Core.Game.Phases.Server;
using Network.Infrastructure;
using Unity.Netcode;
using VContainer;

namespace Network.Game
{
    public class GameStatesNetworkSync : BaseNetworkSync
    {
        private readonly NetworkVariable<ByteData> _destinyCardState = new();
        private readonly NetworkVariable<ByteData> _encounterState = new();
        
        /// <summary>
        /// OnlyServer
        /// </summary>
        private IGameServerDestinyPhaseResolver? _serverDestinyPhaseResolver;
        private IGameServerEncounterManager? _serverEncounterManager;
        
        /// <summary>
        /// Client
        /// </summary>
        private IGameClientDestinyPhaseResolver _clientDestinyPhaseResolver = null!;
        private IGameClientEncounterManager _clientEncounterManager = null!;
        
        /// <summary>
        /// Other
        /// </summary>
        private INetworkSerializer _serializer = null!;
        private IObjectResolver _resolver = null!;

        [Inject]
        private void Constructor(
            IGameClientDestinyPhaseResolver clientDestinyPhaseResolver,
            IGameClientEncounterManager clientEncounterManager,
            
            INetworkSerializer serializer,
            IObjectResolver resolver)
        {
            _clientDestinyPhaseResolver = clientDestinyPhaseResolver;
            _clientEncounterManager = clientEncounterManager;
            _serializer = serializer;
            _resolver = resolver;
        }
        
        protected override void OnNetworkSpawnInternal()
        {
            if (IsServer)
            {
                _serverDestinyPhaseResolver = _resolver.Resolve<IGameServerDestinyPhaseResolver>();
                _serverEncounterManager = _resolver.Resolve<IGameServerEncounterManager>();

                _serverDestinyPhaseResolver.Changed += SendDestinyPhaseState;
                _serverEncounterManager.Started += SendEncounterState;
            }
            else
            {
                _destinyCardState.OnValueChanged += OnDestinyPhaseStateReceived;
                _encounterState.OnValueChanged += OnEncounterStateReceived;
            }
        }

        public override void OnNetworkDespawn()
        {
            if (IsServer)
            {
                if (_serverDestinyPhaseResolver != null)
                {
                    _serverDestinyPhaseResolver.Changed -= SendDestinyPhaseState;
                }
                
                if (_serverEncounterManager != null)
                {
                    _serverEncounterManager.Started -= SendEncounterState;
                }
            }
            else
            {
                _destinyCardState.OnValueChanged -= OnDestinyPhaseStateReceived;
                _encounterState.OnValueChanged -= OnEncounterStateReceived;
            }
        }
        
        private void SendDestinyPhaseState()
        {
            var state = _serverDestinyPhaseResolver!.ToState();
            var bytes = _serializer.Serialize(state);
            _destinyCardState.Value = new ByteData(bytes);
            _clientDestinyPhaseResolver.UpdateState(state);
        }
        
        private void SendEncounterState()
        {
            var state = _serverEncounterManager!.ToState();
            var bytes = _serializer.Serialize(state);
            _encounterState.Value = new ByteData(bytes);
            _clientEncounterManager.UpdateState(state);
        }

        private void OnEncounterStateReceived(ByteData oldValue, ByteData newValue) => 
            ApplyEncounterState(newValue.Data);
        
        private void OnDestinyPhaseStateReceived(ByteData oldValue, ByteData newValue) => 
            ApplyDestinyCardState(newValue.Data);

        private void ApplyEncounterState(byte[]? bytes)
        {
            if (bytes == null || bytes.Length == 0)
            {
                return;
            }

            var state = _serializer.Deserialize<EncounterStateData>(bytes);
            _clientEncounterManager.UpdateState(state);
        } 

        private void ApplyDestinyCardState(byte[]? bytes)
        {
            if (bytes == null || bytes.Length == 0)
            {
                return;
            }
            
            var state = _serializer.Deserialize<DestinyCardStateData>(bytes);
            _clientDestinyPhaseResolver.UpdateState(state);
        }
    }
}