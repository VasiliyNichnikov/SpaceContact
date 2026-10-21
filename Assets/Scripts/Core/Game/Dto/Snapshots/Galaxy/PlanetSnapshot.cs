using System.Collections.Generic;
using System;
using Core.Game.Validation;
using Core.Game.Visibility;
using Newtonsoft.Json;

namespace Core.Game.Dto.Snapshots
{
    [Serializable]
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public struct PlanetSnapshot
    {
        [StateVisibility(StateAudience.Everyone)]
        [StateRequired]
        [StateRange(0, int.MaxValue)]
        [StateIdentity(typeof(PlanetSnapshot))]
        public int PlanetId;

        [StateVisibility(StateAudience.Everyone)]
        [StateRequired]
        [StateUnique]
        [StateReference(typeof(SpaceShipSnapshot))]
        public List<int> ShipIds;
    }
}
