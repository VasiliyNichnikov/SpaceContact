using System.Collections.Generic;
using Core.Game.Dto.Snapshots;

namespace Core.Game
{
    public sealed class EncounterContext : IContext<EncounterSnapshot>
    {
        private readonly List<ulong> _aggressorUnionPlayerIds = new();
        private readonly List<ulong> _defenderUnionPlayerIds = new();

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
            AggressorPlayerId = snapshot.AggressorPlayerId;
            DefenderPlayerId = snapshot.DefenderPlayerId;
            PlanetIdToAttack = snapshot.PlanetIdToAttack;
            
            _defenderUnionPlayerIds.Clear();
            _defenderUnionPlayerIds.AddRange(snapshot.DefenderUnionPlayerIds);
            
            _aggressorUnionPlayerIds.Clear();
            _aggressorUnionPlayerIds.AddRange(snapshot.AggressorUnionPlayerIds);
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
    }
}