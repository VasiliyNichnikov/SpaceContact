using System;
using Core.Game.Events;

namespace Core.Game.Actions
{
    public sealed class GameActionSendResult
    {
        internal GameActionSendResult(
            Guid sourceActionId, 
            bool? isAccepted,
            GameEventBatchApplyResult? batchResult, 
            string? errorMessage, 
            bool isCancelled = false, 
            int? validationCode = null)
        {
            SourceActionId = sourceActionId;
            IsAccepted = isAccepted;
            BatchResult = batchResult;
            ErrorMessage = errorMessage;
            IsCancelled = isCancelled;
            ValidationCode = validationCode;
        }

        public Guid SourceActionId { get; }

        // Null means that no matching server response was received.
        public bool? IsAccepted { get; }

        public GameEventBatchApplyResult? BatchResult { get; }

        public string? ErrorMessage { get; }

        // Present only for a server-side gameplay validation failure; interpret it for the source action.
        public int? ValidationCode { get; }

        public bool IsCancelled { get; }
    }
}
