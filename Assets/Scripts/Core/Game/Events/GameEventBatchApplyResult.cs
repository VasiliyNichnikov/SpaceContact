namespace Core.Game.Events
{
    public sealed class GameEventBatchApplyResult
    {
        internal GameEventBatchApplyResult(GameEventBatchApplyStatus status, long revision, string? errorMessage = null)
        {
            Status = status;
            Revision = revision;
            ErrorMessage = errorMessage;
        }

        public GameEventBatchApplyStatus Status { get; }

        public long Revision { get; }

        public string? ErrorMessage { get; }
    }
}
