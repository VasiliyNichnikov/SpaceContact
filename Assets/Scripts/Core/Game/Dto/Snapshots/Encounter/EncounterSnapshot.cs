using System.Collections.Generic;
using System;
using Core.Game.Validation;
using Core.Game.Visibility;
using Newtonsoft.Json;

namespace Core.Game.Dto.Snapshots
{
    /// <summary>
    /// Stores information about the collision between the attacking side
    /// and the defender
    /// </summary>
    [Serializable]
    [StateRule(typeof(EncounterDependenciesRule))]
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class EncounterSnapshot
    {
        [StateVisibility(StateAudience.Everyone)]
        [StateReference(typeof(GamePlayerSnapshot))]
        public ulong? AggressorPlayerId;

        [StateVisibility(StateAudience.Everyone)]
        [StateReference(typeof(GamePlayerSnapshot))]
        public ulong? DefenderPlayerId;

        [StateVisibility(StateAudience.Everyone)]
        [StateReference(typeof(PlanetSnapshot))]
        public int? PlanetIdToAttack;

        [StateVisibility(StateAudience.Everyone)]
        [StateRequired]
        [StateUnique]
        [StateReference(typeof(GamePlayerSnapshot))]
        public List<ulong> AggressorUnionPlayerIds = new();

        [StateVisibility(StateAudience.Everyone)]
        [StateRequired]
        [StateUnique]
        [StateReference(typeof(GamePlayerSnapshot))]
        public List<ulong> DefenderUnionPlayerIds = new();
    }
}
