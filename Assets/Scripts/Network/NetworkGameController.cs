using System;
using Core;
using Core.Phases;
using Logs;
using Network.Infrastructure;
using Unity.Netcode;
using VContainer;

namespace Network
{
    public class NetworkGameController : NetworkBehaviour
    {
        private GameStateMachine _stateMachine = null!;
        private PhaseRegistry _phaseRegistry = null!;
        private INetworkSerializer _serializer = null!;
        
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
            
            SetPhaseClientRpc(phaseId, dataBytes);
        }
        
        [ClientRpc]
        private void SetPhaseClientRpc(byte phaseId, byte[]? dataBytes)
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