using System;
using System.Collections.Generic;

namespace Core.Game.Dto.Snapshots
{
    /// <summary>
    /// Stores information about the collision between the attacking side
    /// and the defender
    /// </summary>
    [Serializable]
    public class EncounterSnapshot
    {
        public ulong? AggressorPlayerId;
        
        public ulong? DefenderPlayerId;
        
        public int? PlanetIdToAttack;

        public List<ulong> AggressorUnionPlayerIds = new();

        public List<ulong> DefenderUnionPlayerIds = new();
    }
}