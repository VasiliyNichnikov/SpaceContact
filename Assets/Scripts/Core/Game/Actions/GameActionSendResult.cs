using System;
using Core.Game.Events;

namespace Core.Game.Actions
{
    public sealed class GameActionSendResult
    {
        internal GameActionSendResult(Guid sourceActionId, bool? isAccepted,
            GameEventBatchApplyResult? batchResult, string? errorMessage, bool isCancelled = false)
        {
            SourceActionId = sourceActionId;
            IsAccepted = isAccepted;
            BatchResult = batchResult;
            ErrorMessage = errorMessage;
            IsCancelled = isCancelled;
        }

        public Guid SourceActionId { get; }

        // Null means that no matching server response was received.
        public bool? IsAccepted { get; }

        public GameEventBatchApplyResult? BatchResult { get; }

        public string? ErrorMessage { get; }

        public bool IsCancelled { get; }
    }
}
