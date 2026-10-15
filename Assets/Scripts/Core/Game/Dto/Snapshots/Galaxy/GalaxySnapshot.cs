using System.Collections.Generic;
using System;
using Core.Game.Visibility;
using Newtonsoft.Json;

namespace Core.Game.Dto.Snapshots
{
    [Serializable]
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public sealed class GalaxySnapshot
    {
        /// <summary>
        /// Key - player id,
        /// Value - owned planets
        /// </summary>
        [StateVisibility(StateAudience.Everyone)]
        public Dictionary<ulong, List<PlanetSnapshot>> Planets = new();

        /// <summary>
        /// Key - player id,
        /// Value - owned ships
        /// </summary>
        [StateVisibility(StateAudience.Everyone)]
        public Dictionary<ulong, List<SpaceShipSnapshot>> Ships = new();
    }
}
