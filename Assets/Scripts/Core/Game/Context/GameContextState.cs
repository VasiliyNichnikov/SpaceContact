using System;

namespace Core.Game
{
    internal struct GameContextState
    {
        private Operation _operation;

        public GameContextState(GameContextRole role) : this()
        {
            Role = role;
        }

        private enum Operation
        {
            None,
            ServerOperation,
            ClientBatch
        }

        public GameContextRole Role { get; }

        public long Revision { get; private set; }

        public bool IsInitialized { get; private set; }

        public bool IsOperationFaulted { get; private set; }

        public bool IsNotifying { get; private set; }

        public long? RequiredSnapshotRevision { get; private set; }

        public bool IsApplyingServerOperation => _operation == Operation.ServerOperation;

        public bool IsApplyingClientBatch => _operation == Operation.ClientBatch;

        public bool IsBusy => IsNotifying || _operation != Operation.None;

        public bool IsSynchronizationRequired =>
            Role == GameContextRole.Client && (!IsInitialized || RequiredSnapshotRevision.HasValue);

        public bool CanCreateSnapshot =>
            IsInitialized && !IsOperationFaulted && !IsSynchronizationRequired &&
            (_operation == Operation.None || IsNotifying);

        public void LoadSnapshot(long revision)
        {
            Revision = revision;
            IsInitialized = true;
            RequiredSnapshotRevision = null;
        }

        public void CommitRevision(long revision)
        {
            Revision = revision;
        }

        public void RequireClientSnapshot(long revision)
        {
            // Keep the first recovery boundary stable while newer live batches are waiting outside Core.
            RequiredSnapshotRevision ??= Math.Max(Revision, revision);
        }

        public void BeginClientBatch()
        {
            _operation = Operation.ClientBatch;
        }

        public void BeginServerOperation()
        {
            _operation = Operation.ServerOperation;
        }

        public void FaultServerOperation()
        {
            IsOperationFaulted = true;
        }

        public void EndOperation()
        {
            _operation = Operation.None;
        }

        public void BeginNotification()
        {
            IsNotifying = true;
        }

        public void EndNotification()
        {
            IsNotifying = false;
        }
    }
}
