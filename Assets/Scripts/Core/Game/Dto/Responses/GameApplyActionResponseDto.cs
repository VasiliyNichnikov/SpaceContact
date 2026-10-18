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

        [JsonConstructor]
        private GameApplyActionResponseDto(Guid sourceActionId, GameEventBatchData? batch, string? errorMessage)
        {
            SourceActionId = sourceActionId;
            Batch = batch;
            ErrorMessage = errorMessage;
        }

        public bool IsAccepted => Batch != null;

        public static GameApplyActionResponseDto Rejected(Guid actionId, string? errorMessage) =>
            new(actionId, null, errorMessage);

        public static GameApplyActionResponseDto Accepted(Guid actionId, GameEventBatchData batch) =>
            new(actionId, batch, null);
    }
}
