using System.Collections.Generic;
using System;
using Core.Game.Visibility;
using Newtonsoft.Json;

namespace Core.Game.Dto.Snapshots
{
    [Serializable]
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public struct PlanetSnapshot
    {
        [StateVisibility(StateAudience.Everyone)]
        public int PlanetId;

        [StateVisibility(StateAudience.Everyone)]
        public List<int> ShipIds;
    }
}
