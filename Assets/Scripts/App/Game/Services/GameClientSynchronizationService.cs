using System;
using Network.Game;

namespace App.Game.Services
{
    public sealed class GameClientSynchronizationService : IDisposable
    {
        private readonly GameStartupNetwork _startupNetwork;
        private readonly GameSnapshotRecovery _recovery;
        private bool _disposed;

        public GameClientSynchronizationService(GameStartupNetwork startupNetwork, GameSnapshotRecovery recovery)
        {
            _startupNetwork = startupNetwork;
            _recovery = recovery;
        }

        public void Start()
        {
            _recovery.Start();
            _startupNetwork.SnapshotSyncAllowed += OnSnapshotSyncAllowed;
            if (_startupNetwork.IsSnapshotSynchronizationAllowed)
            {
                OnSnapshotSyncAllowed();
            }
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            _startupNetwork.SnapshotSyncAllowed -= OnSnapshotSyncAllowed;
            _recovery.Stop();
        }

        private void OnSnapshotSyncAllowed()
        {
            if (!_disposed)
            {
                _recovery.AllowSynchronization();
            }
        }
    }
}
