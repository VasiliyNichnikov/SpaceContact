using System;
using System.Collections.Generic;
using Core.Game.Dto.Snapshots;

namespace Core.Game
{
    public sealed class EncounterContext : IContext<EncounterSnapshot>
    {
        private List<ulong> _aggressorUnionPlayerIds = new();
        private List<ulong> _defenderUnionPlayerIds = new();

        private ulong? _defenderPlayerId;
        private int? _planetIdToAttack;

        public event Action? OnDefenderPlayerChanged;

        public event Action? OnPlanetIdChanged;

        public ulong? AggressorPlayerId { get; private set; }

        public ulong? DefenderPlayerId
        {
            get => _defenderPlayerId;
            private set
            {
                if (_defenderPlayerId == value)
                {
                    return;
                }

                _defenderPlayerId = value;
                OnDefenderPlayerChanged?.Invoke();
            }
        }

        public int? PlanetIdToAttack
        {
            get => _planetIdToAttack;
            private set
            {
                if (_planetIdToAttack == value)
                {
                    return;
                }

                _planetIdToAttack = value;

                OnPlanetIdChanged?.Invoke();
            }
        }

        public bool HasAggressor =>
            AggressorPlayerId != null;

        public bool HasDefender =>
            DefenderPlayerId != null;

        public bool HasPlanetToAttack =>
            PlanetIdToAttack != null;

        public void SetAggressor(ulong playerId)
        {
            AggressorPlayerId = playerId;
            ResetTarget();
        }

        public void ResetTarget()
        {
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
