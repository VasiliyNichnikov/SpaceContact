using System;

namespace Core.Game.Dto.States
{
    /// <summary>
    /// Хранит информацию о столкновение между атакующей стороной
    /// и обороняющийся
    /// </summary>
    [Serializable]
    public class EncounterStateData
    {
        public bool HasAggressorPlayerId;
        
        public ulong AggressorPlayerId;

        public bool HasDefenderPlayerId;
        
        public ulong DefenderPlayerId;

        public bool HasPlanetToAttack;
        
        public int PlanetIdToAttack;

        public ulong[] AggressorUnionPlayerIds = Array.Empty<ulong>();
        
        public ulong[] DefenderUnionPlayerIds = Array.Empty<ulong>();
    }
}