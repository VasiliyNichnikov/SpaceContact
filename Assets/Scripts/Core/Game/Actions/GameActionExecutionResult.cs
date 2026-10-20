using System.Collections.Generic;
using System.Collections.ObjectModel;
using Core.Game.Dto;

namespace Core.Game.Actions
{
    public sealed class GameActionExecutionResult
    {
        private GameActionExecutionResult(bool isSuccess, string? errorMessage, long revision, bool hasChanges,
            bool isDuplicate, IReadOnlyDictionary<ulong, GameEventBatchData> batchesByRecipient, int? validationCode = null)
        {
            IsSuccess = isSuccess;
            ErrorMessage = errorMessage;
            Revision = revision;
            HasChanges = hasChanges;
            IsDuplicate = isDuplicate;
            BatchesByRecipient = batchesByRecipient;
            ValidationCode = validationCode;
        }

        public bool IsSuccess { get; }

        public string? ErrorMessage { get; }

        public int? ValidationCode { get; }

        public long Revision { get; }

        public bool HasChanges { get; }

        public bool IsDuplicate { get; }

        public IReadOnlyDictionary<ulong, GameEventBatchData> BatchesByRecipient { get; }

        internal static GameActionExecutionResult Rejected(long revision, string? errorMessage, int? validationCode = null)
        {
            return new GameActionExecutionResult(false, errorMessage, revision, false, false,
                new ReadOnlyDictionary<ulong, GameEventBatchData>(new Dictionary<ulong, GameEventBatchData>()), validationCode);
        }

        internal static GameActionExecutionResult Accepted(long revision, bool hasChanges,
            Dictionary<ulong, GameEventBatchData> batchesByRecipient)
        {
            return new GameActionExecutionResult(true, null, revision, hasChanges, false,
                new ReadOnlyDictionary<ulong, GameEventBatchData>(batchesByRecipient));
        }

        internal GameActionExecutionResult AsDuplicate()
        {
            return new GameActionExecutionResult(IsSuccess, ErrorMessage, Revision, HasChanges, true, BatchesByRecipient, ValidationCode);
        }
    }
}
