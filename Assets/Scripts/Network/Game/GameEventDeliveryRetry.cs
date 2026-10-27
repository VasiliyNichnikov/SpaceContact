using Core.Game.Dto;

namespace Network.Game
{
    internal sealed class GameEventDeliveryRetry
    {
        public GameEventDeliveryRetry(GameEventBatchData batch, double nextAttemptTimeInSeconds, string lastError)
        {
            Batch = batch;
            NextAttemptTimeInSeconds = nextAttemptTimeInSeconds;
            LastError = lastError;
        }

        public GameEventBatchData Batch { get; }

        public int RetryCount { get; set; }

        public double NextAttemptTimeInSeconds { get; set; }

        public string LastError { get; set; }
    }
}
