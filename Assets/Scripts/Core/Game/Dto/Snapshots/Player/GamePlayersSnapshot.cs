using System.Collections.Generic;
using System;
using Core.Game.Visibility;
using Newtonsoft.Json;

namespace Core.Game.Dto.Snapshots
{
    [Serializable]
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public sealed class GamePlayersSnapshot
    {
        [StateVisibility(StateAudience.Everyone)]
        public List<GamePlayerSnapshot> Players = new();
    }
}
