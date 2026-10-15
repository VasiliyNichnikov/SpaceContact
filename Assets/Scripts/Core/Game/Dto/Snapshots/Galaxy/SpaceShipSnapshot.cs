using System;
using Core.Game.Visibility;
using Newtonsoft.Json;

namespace Core.Game.Dto.Snapshots
{
    [Serializable]
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public struct SpaceShipSnapshot
    {
        [StateVisibility(StateAudience.Everyone)]
        public int ShipId;
    }
}
