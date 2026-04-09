using System;
using Core.Game.Dto.States;
using Core.Game.Players;

namespace Core.Game.Encounter
{
    public sealed class GameClientEncounterManager : IGameClientEncounterManager, IGameClientEncounterEvents
    {
        private readonly GamePlayersRegistry _playersRegistry;
        
        private IGamePlayer? _aggressorPlayer;
        private IGamePlayer? _defenderPlayer;
        
        public GameClientEncounterManager(GamePlayersRegistry playersRegistry)
        {
            _playersRegistry = playersRegistry;
        }
        
        public event Action? StateChanged;
        
        public event Action? AggressorChanged;
        
        public event Action? DefenderChanged;

        IGamePlayer? IGameClientEncounterManager.AggressorPlayer => 
            _aggressorPlayer;

        IGamePlayer? IGameClientEncounterManager.DefenderPlayer => 
            _defenderPlayer;

        void IGameClientEncounterEvents.SetAggressorEvent(ulong aggressorPlayerId)
        {
            _aggressorPlayer = _playersRegistry.GetPlayerById(aggressorPlayerId);
            AggressorChanged?.Invoke();
        }
        
        void IGameClientEncounterEvents.SetDefenderEvent(ulong defenderPlayerId)
        {
            _defenderPlayer = _playersRegistry.GetPlayerById(defenderPlayerId);
            DefenderChanged?.Invoke();
        }
        
        public void UpdateState(EncounterStateData state)
        {
            _aggressorPlayer = state.HasAggressorPlayerId 
                ? _playersRegistry.GetPlayerById(state.AggressorPlayerId) 
                : null;
            
            _defenderPlayer = state.HasDefenderPlayerId
                ? _playersRegistry.GetPlayerById(state.DefenderPlayerId)
                : null;
            
            StateChanged?.Invoke();
        }
    }
}