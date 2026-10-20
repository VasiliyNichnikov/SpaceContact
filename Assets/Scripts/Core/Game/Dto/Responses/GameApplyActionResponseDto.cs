using System;
using Core.Game.Validation;
using Newtonsoft.Json;

namespace Core.Game.Dto
{
    [Serializable]
    public class GameApplyActionResponseDto
    {
        [StateRequired]
        public Guid SourceActionId;

        public GameEventBatchData? Batch;

        public string? ErrorMessage;

        // Infrastructure failures have no gameplay validation code.
        public int? ValidationCode;

        [JsonConstructor]
        private GameApplyActionResponseDto(
            Guid sourceActionId, 
            GameEventBatchData? batch, 
            string? errorMessage,
            int? validationCode = null)
        {
            SourceActionId = sourceActionId;
            Batch = batch;
            ErrorMessage = errorMessage;
            ValidationCode = validationCode;
        }

        public bool IsAccepted => Batch != null;

        public static GameApplyActionResponseDto Rejected(Guid actionId, string? errorMessage, int? validationCode = null) =>
            new(actionId, null, errorMessage, validationCode);

        public static GameApplyActionResponseDto Accepted(Guid actionId, GameEventBatchData batch) =>
            new(actionId, batch, null);
    }
}
