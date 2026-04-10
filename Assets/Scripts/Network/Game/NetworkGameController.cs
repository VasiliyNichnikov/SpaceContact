using System;
using System.Threading.Tasks;
using Core.Game;
using Core.Game.Phases;
using GeneralUtils;
using Logs;
using Network.Infrastructure;
using Unity.Netcode;
using VContainer;

namespace Network.Game
{
    public class NetworkGameController : NetworkBehaviour, IServerStateMachineNetwork
    {
        private readonly struct CachePhase
        {
            public readonly byte StateId;

            public readonly byte[]? Payload;

            public CachePhase(byte stateId, byte[]? payload)
            {
                StateId = stateId;
                Payload = payload;
            }
        }
        
        private GameStateMachine _stateMachine = null!;
        private PhaseRegistry _phaseRegistry = null!;
        private INetworkSerializer _serializer = null!;
        private GamePlayersPhaseTracker _tracker = null!;
        
        private Action? _pendingTransition;
        private CachePhase? _cachePhase;
        
        [Inject]
        private void Constructor(
            GameStateMachine stateMachine, 
            PhaseRegistry phaseRegistry, 
            INetworkSerializer serializer,
            GamePlayersPhaseTracker tracker)
        {
            _stateMachine = stateMachine;
            _phaseRegistry = phaseRegistry;
            _serializer = serializer;
            _tracker = tracker;
        }

        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            }
        }

        public override void OnNetworkDespawn()
        {
            if (IsServer && NetworkManager.Singleton != null)
            {
                NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            }
        }

        void IServerStateMachineNetwork.ServerTransitionTo<TPhase>(IPhasePayload? payload)
        {
            if (!IsServer)
            {
                return;
            }

            if (_pendingTransition != null)
            {
                Logger.Error($"{nameof(NetworkGameController)}.{nameof(ReportPlayerStateChangedRpc)}: the transition has already started.");
                return;
            }
            
            var phaseId = _phaseRegistry.GetId<TPhase>();

            var dataBytes = payload != null
                ? _serializer.Serialize(payload)
                : Array.Empty<byte>();

            _cachePhase = new CachePhase(phaseId, dataBytes);
            _tracker.ChangePhase(NetworkManager.LocalClientId, phaseId);
            _pendingTransition = () => _stateMachine.TransitionTo(typeof(TPhase), payload).FireAndForget();
            SetPhaseClientRpc(phaseId, dataBytes);
        }

        private void OnClientConnected(ulong clientId)
        {
            if (_cachePhase == null)
            {
                return;
            }

            var rcpParams = new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new[] { clientId }
                }
            };
            
            Logger.Log($"{nameof(NetworkGameController)}.{nameof(OnClientConnected)}: syncing phase {_cachePhase.Value.StateId} to new client {clientId}.");
            SetPhaseClientRpc(_cachePhase.Value.StateId, _cachePhase.Value.Payload, rcpParams);
        }
        
        [ClientRpc]
        private void SetPhaseClientRpc(byte phaseId, byte[]? dataBytes, ClientRpcParams _ = default)
        {
            if (IsServer)
            {
                return;
            }

            try
            {
                var phaseType = _phaseRegistry.GetPhaseType(phaseId);
                var dataType = _phaseRegistry.GetDataType(phaseType);
                IPhasePayload? payload = null;

                if (dataType != null && dataBytes is { Length: > 0 })
                {
                    var objectRaw = _serializer.Deserialize(dataType, dataBytes);
                    payload = objectRaw as IPhasePayload;

                    if (payload == null && objectRaw != null!)
                    {
                        Logger.Error($"{nameof(NetworkGameController)}.{nameof(SetPhaseClientRpc)}: invalid data conversion.");
                    }
                }
                
                TransitionToStateAsync(phaseId, phaseType, payload).FireAndForget();
            }
            catch (Exception e)
            {
                Logger.Error($"NetworkGameController.SetPhaseClientRpc: during data transfer: {e.Message}.");
            }
        }

        private async Task TransitionToStateAsync(byte phaseId, Type phaseType, IPhasePayload? payload)
        {
            await _stateMachine.TransitionTo(phaseType, payload);
            ReportPlayerStateChangedRpc(phaseId);
        }

        [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
        private void ReportPlayerStateChangedRpc(int phaseId, RpcParams rpcParams = default)
        {
            if (!IsServer)
            {
                return;
            }
            
            var playerId = rpcParams.Receive.SenderClientId;
            _tracker.ChangePhase(playerId, phaseId);

            if (!_tracker.AreAllPlayersInPhase(phaseId))
            {
                return;
            }
            
            if (_pendingTransition == null)
            {
                Logger.Error($"{nameof(NetworkGameController)}.{nameof(ReportPlayerStateChangedRpc)}: no pending transition.");
                    
                return;
            }
            
            var pendingTransitionAction = _pendingTransition;
            _pendingTransition = null;
            pendingTransitionAction.Invoke();
        }
    }
}