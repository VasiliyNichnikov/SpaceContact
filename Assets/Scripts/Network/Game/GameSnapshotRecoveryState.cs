namespace Network.Game
{
    internal struct GameSnapshotRecoveryState
    {
        public bool IsStarted { get; set; }

        public bool IsDisposed { get; set; }

        public bool IsSynchronizationAllowed { get; set; }

        public bool IsRecovering { get; set; }
    }
}
