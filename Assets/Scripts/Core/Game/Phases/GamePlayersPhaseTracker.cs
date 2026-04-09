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
        
        public void Init(IReadOnlyCollection<IGamePlayer> players) =>
            _currentPhaseIdByPlayerId = CreatePhaseByPlayerId(players);
        
        public bool AreAllPlayersInPhase(GamePhaseType phaseType) => 
            _currentPhaseIdByPlayerId != null && 
            _currentPhaseIdByPlayerId.Values.All(phaseId => GamePhaseConvertor.ToPhaseType(phaseId) == phaseType);

        public bool AreAllPlayersInPhase(int phaseId) =>
            _currentPhaseIdByPlayerId != null &&
            _currentPhaseIdByPlayerId.Values.All(p => p == phaseId);

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
            Logger.Log($"GamePlayerPhaseTracker.ChangePhase: Player {playerId} change phase to {phaseId}.");
        }

        private static Dictionary<ulong, int> CreatePhaseByPlayerId(IReadOnlyCollection<IGamePlayer> players) => 
            players.ToDictionary(player => player.PlayerId, _ => PhaseIds.InvalidPhaseId);
    }
}