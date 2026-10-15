using System;
using Core.EngineData;
using Core.Game.Visibility;
using Newtonsoft.Json;

namespace Core.Game.Dto.Snapshots
{
    [Serializable]
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public struct GamePlayerSnapshot
    {
        [StateVisibility(StateAudience.Everyone, IsOwnerId = true)]
        public ulong PlayerId;

        [StateVisibility(StateAudience.Everyone)]
        public string Name;

        [StateVisibility(StateAudience.Everyone)]
        public int Order;

        [StateVisibility(StateAudience.Everyone)]
        public Color Color;

        [StateVisibility(StateAudience.Everyone)]
        public bool IsReady;
    }
}
