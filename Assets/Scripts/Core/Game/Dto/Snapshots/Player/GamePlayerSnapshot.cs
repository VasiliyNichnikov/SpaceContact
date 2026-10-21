using System;
using Core.EngineData;
using Core.Game.Validation;
using Core.Game.Visibility;
using Newtonsoft.Json;

namespace Core.Game.Dto.Snapshots
{
    [Serializable]
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public struct GamePlayerSnapshot
    {
        [StateVisibility(StateAudience.Everyone, IsOwnerId = true)]
        [StateRequired]
        [StateIdentity(typeof(GamePlayerSnapshot))]
        public ulong PlayerId;

        [StateVisibility(StateAudience.Everyone)]
        [StateRequired]
        [StateNotEmpty]
        public string Name;

        [StateVisibility(StateAudience.Everyone)]
        [StateRequired]
        [StateRange(0, int.MaxValue)]
        public int Order;

        [StateVisibility(StateAudience.Everyone)]
        [StateRequired]
        public Color Color;

        [StateVisibility(StateAudience.Everyone)]
        [StateRequired]
        public bool IsReady;
    }
}
