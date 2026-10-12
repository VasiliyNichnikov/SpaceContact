using System.Collections.Generic;
using Core.Game.Dto.Snapshots;

namespace Core.Game
{
    public sealed class EncounterContext : IContext<EncounterSnapshot>
    {
        private List<ulong> _aggressorUnionPlayerIds = new();
        private List<ulong> _defenderUnionPlayerIds = new();

        public ulong? AggressorPlayerId { get; private set; }

        public ulong? DefenderPlayerId { get; private set; }
        
        public int? PlanetIdToAttack { get; private set; }
        
        public bool HasAggressor => 
            AggressorPlayerId != null;
        
        public bool HasDefender => 
            DefenderPlayerId != null;

        public bool HasPlanetToAttack => 
            PlanetIdToAttack != null;

        public void SetAggressor(ulong playerId)
        {
            AggressorPlayerId = playerId;
            DefenderPlayerId = null;
            PlanetIdToAttack = null;
            _aggressorUnionPlayerIds.Clear();
            _defenderUnionPlayerIds.Clear();
        }

        public void SetDefender(ulong playerId)
        {
            DefenderPlayerId = playerId;
            _defenderUnionPlayerIds.Clear();
            _defenderUnionPlayerIds.Add(playerId);
        }

        public void SetPlanetToAttack(int planetId)
        {
            PlanetIdToAttack = planetId;
        }

        public void ClearDefender()
        {
            DefenderPlayerId = null;
            _defenderUnionPlayerIds.Clear();
        }

        public void ClearPlanetToAttack()
        {
            PlanetIdToAttack = null;
        }

        public void LoadSnapshot(EncounterSnapshot snapshot)
        {
            var aggressorUnionPlayerIds = new List<ulong>(snapshot.AggressorUnionPlayerIds);
            var defenderUnionPlayerIds = new List<ulong>(snapshot.DefenderUnionPlayerIds);

            AggressorPlayerId = snapshot.AggressorPlayerId;
            DefenderPlayerId = snapshot.DefenderPlayerId;
            PlanetIdToAttack = snapshot.PlanetIdToAttack;
            
            _defenderUnionPlayerIds = defenderUnionPlayerIds;
            _aggressorUnionPlayerIds = aggressorUnionPlayerIds;
        }

        public EncounterSnapshot ToSnapshot()
        {
            var snapshot = new EncounterSnapshot
            {
                AggressorPlayerId = AggressorPlayerId,
                DefenderPlayerId = DefenderPlayerId,
                PlanetIdToAttack = PlanetIdToAttack
            };
            
            snapshot.DefenderUnionPlayerIds.AddRange(_defenderUnionPlayerIds);
            snapshot.AggressorUnionPlayerIds.AddRange(_aggressorUnionPlayerIds);
            
            return snapshot;
        }

        internal void ReplaceState(EncounterContext prepared)
        {
            AggressorPlayerId = prepared.AggressorPlayerId;
            DefenderPlayerId = prepared.DefenderPlayerId;
            PlanetIdToAttack = prepared.PlanetIdToAttack;
            _aggressorUnionPlayerIds = prepared._aggressorUnionPlayerIds;
            _defenderUnionPlayerIds = prepared._defenderUnionPlayerIds;
        }
    }
}
