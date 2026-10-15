using System.Collections.Generic;
using System;
using Core.Game.Visibility;
using Newtonsoft.Json;

namespace Core.Game.Dto.Snapshots
{
    /// <summary>
    /// Stores information about the collision between the attacking side
    /// and the defender
    /// </summary>
    [Serializable]
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class EncounterSnapshot
    {
        [StateVisibility(StateAudience.Everyone)]
        public ulong? AggressorPlayerId;

        [StateVisibility(StateAudience.Everyone)]
        public ulong? DefenderPlayerId;

        [StateVisibility(StateAudience.Everyone)]
        public int? PlanetIdToAttack;

        [StateVisibility(StateAudience.Everyone)]
        public List<ulong> AggressorUnionPlayerIds = new();

        [StateVisibility(StateAudience.Everyone)]
        public List<ulong> DefenderUnionPlayerIds = new();
    }
}
