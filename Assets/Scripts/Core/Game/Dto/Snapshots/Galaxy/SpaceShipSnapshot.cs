using System;
using Core.Game.Validation;
using Core.Game.Visibility;
using Newtonsoft.Json;

namespace Core.Game.Dto.Snapshots
{
    [Serializable]
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public struct SpaceShipSnapshot
    {
        [StateVisibility(StateAudience.Everyone)]
        [StateRequired]
        [StateRange(0, int.MaxValue)]
        [StateIdentity(typeof(SpaceShipSnapshot))]
        public int ShipId;
    }
}
