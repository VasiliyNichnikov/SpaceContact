using System.Collections.Generic;
using System;
using Core.Game.Validation;
using Core.Game.Visibility;
using Newtonsoft.Json;

namespace Core.Game.Dto.Snapshots
{
    [Serializable]
    [StateRule(typeof(ShipPlacementRule))]
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public sealed class GalaxySnapshot
    {
        /// <summary>
        /// Key - player id,
        /// Value - owned planets
        /// </summary>
        [StateVisibility(StateAudience.Everyone)]
        [StateRequired]
        [StateReference(typeof(GamePlayerSnapshot), MatchAllIdentities = true)]
        public Dictionary<ulong, List<PlanetSnapshot>> Planets = new();

        /// <summary>
        /// Key - player id,
        /// Value - owned ships
        /// </summary>
        [StateVisibility(StateAudience.Everyone)]
        [StateRequired]
        [StateReference(typeof(GamePlayerSnapshot), MatchAllIdentities = true)]
        public Dictionary<ulong, List<SpaceShipSnapshot>> Ships = new();
    }
}
