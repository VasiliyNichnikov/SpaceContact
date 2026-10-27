using System;
using Unity.Netcode;

namespace Network.Game
{
    public sealed class GameStartupNetwork : NetworkBehaviour
    {
        private readonly NetworkVariable<GameStartupStage> _stage = new(
            GameStartupStage.Loading, 
            NetworkVariableReadPermission.Everyone, 
            NetworkVariableWritePermission.Server);

        public event Action? SnapshotSyncAllowed;
        
        public bool IsSnapshotSynchronizationAllowed =>
            IsClient && IsSpawned && IsSynchronizationStage(_stage.Value);

        public bool IsWaitingForInitialAcknowledgements =>
            IsServer && IsSpawned && _stage.Value == GameStartupStage.SnapshotSyncAllowed;

        public override void OnNetworkSpawn()
        {
            if (!IsClient)
            {
                return;
            }

            _stage.OnValueChanged += OnStageChanged;
            ApplyStage(_stage.Value);
        }

        public override void OnNetworkDespawn()
        {
            _stage.OnValueChanged -= OnStageChanged;
        }

        public void AllowSnapshotSynchronization()
        {
            if (!IsServer || !IsSpawned || _stage.Value != GameStartupStage.Loading)
            {
                return;
            }
            
            _stage.Value = GameStartupStage.SnapshotSyncAllowed;
        }

        public void MarkGameRunning()
        {
            if (!IsWaitingForInitialAcknowledgements)
            {
                return;
            }

            _stage.Value = GameStartupStage.Running;
        }

        private static bool IsSynchronizationStage(GameStartupStage stage)
        {
            return stage is GameStartupStage.SnapshotSyncAllowed or GameStartupStage.Running;
        }

        private void OnStageChanged(GameStartupStage previous, GameStartupStage current)
        {
            ApplyStage(current);
        }

        private void ApplyStage(GameStartupStage stage)
        {
            if (IsSynchronizationStage(stage))
            {
                SnapshotSyncAllowed?.Invoke();
            }
        }
    }
}