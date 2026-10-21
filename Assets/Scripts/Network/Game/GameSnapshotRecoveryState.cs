namespace Network.Game
{
    internal struct GameSnapshotRecoveryState
    {
        public bool IsStarted { get; set; }

        public bool IsStopped { get; set; }

        public bool IsSynchronizationAllowed { get; set; }

        public bool IsRecovering { get; set; }
    }
}
