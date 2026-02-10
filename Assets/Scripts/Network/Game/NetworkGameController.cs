using System;
using Core.Game;
using Core.Game.Phases;
using Logs;
using Network.Infrastructure;
using Unity.Netcode;
using VContainer;

namespace Network.Game
{
    public class NetworkGameController : NetworkBehaviour
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

        private CachePhase? _cachePhase;
        
        [Inject]
        private void Constructor(
            GameStateMachine stateMachine, 
            PhaseRegistry phaseRegistry, 
            INetworkSerializer serializer)
        {
            _stateMachine = stateMachine;
            _phaseRegistry = phaseRegistry;
            _serializer = serializer;
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

        public void ServerTransitionTo<TPhase>(IPhasePayload? payload = null) 
            where TPhase : IGamePhase
        {
            if (!IsServer)
            {
                return;
            }
            
            _stateMachine.TransitionTo(typeof(TPhase), payload);
            var phaseId = _phaseRegistry.GetId<TPhase>();

            var dataBytes = payload != null
                ? _serializer.Serialize(payload)
                : Array.Empty<byte>();

            _cachePhase = new CachePhase(phaseId, dataBytes);
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
                Send = new ClientRpcSendParams()
                {
                    TargetClientIds = new[] { clientId }
                }
            };
            
            Logger.Log($"NetworkGameController.OnClientConnected: syncing phase {_cachePhase.Value.StateId} to new client {clientId}.");
            SetPhaseClientRpc(_cachePhase.Value.StateId, _cachePhase.Value.Payload, rcpParams);
        }
        
        [ClientRpc]
        private void SetPhaseClientRpc(byte phaseId, byte[]? dataBytes, ClientRpcParams rpcParams = default)
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
                        Logger.Error("NetworkGameController.SetPhaseClientRpc: invalid data conversion.");
                    }
                }
                
                _stateMachine.TransitionTo(phaseType, payload);
            }
            catch (Exception e)
            {
                Logger.Error($"NetworkGameController.SetPhaseClientRpc: during data transfer: {e.Message}.");
            }
        }
    }
}