using System;
using System.Collections.Generic;
using System.Linq;
using Core.Game.Players;
using Logs;

namespace Core.Game.Phases
{
    public sealed class GamePlayersPhaseTracker
    {
        private Dictionary<ulong, int>? _currentPhaseIdByPlayerId;

        public event Action<ulong>? PlayerPhaseChanged; 
        
        public void Init(IReadOnlyCollection<IGamePlayer> players) =>
            _currentPhaseIdByPlayerId = CreatePhaseByPlayerId(players);
        
        public bool AreAllPlayersInPhase(GamePhaseType phaseType) => 
            _currentPhaseIdByPlayerId != null && 
            _currentPhaseIdByPlayerId.Values.All(phaseId => GamePhaseConvertor.ToPhaseType(phaseId) == phaseType);

        public GamePhaseType GetPlayerPhase(ulong playerId)
        {
            var phaseId = _currentPhaseIdByPlayerId.GetValueOrDefault(playerId, PhaseIds.InvalidPhaseId);
            
            return GamePhaseConvertor.ToPhaseType(phaseId);
        }

        public void ChangePhase(ulong playerId, int phaseId)
        {
            if (_currentPhaseIdByPlayerId == null)
            {
                Logger.Error("PlayerPhaseTracker.ChangePhase: currentPhaseByPlayerId is null.");
                
                return;
            }

            var currentPhase = _currentPhaseIdByPlayerId.GetValueOrDefault(playerId);

            if (currentPhase == phaseId)
            {
                Logger.Error("PlayerPhaseTracker.ChangePhase: new phase equals the current one.");
                
                return;
            }
            
            _currentPhaseIdByPlayerId[playerId] = phaseId;
            PlayerPhaseChanged?.Invoke(playerId);
            Logger.Log($"GamePlayerPhaseTracker.ChangePhase: Player {playerId} change phase to {phaseId}.");
        }

        private static Dictionary<ulong, int> CreatePhaseByPlayerId(IReadOnlyCollection<IGamePlayer> players) => 
            players.ToDictionary(player => player.PlayerId, _ => PhaseIds.InvalidPhaseId);
    }
}