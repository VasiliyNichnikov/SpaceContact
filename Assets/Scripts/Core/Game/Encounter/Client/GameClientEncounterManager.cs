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
        private int? _planetIdToAttack;
        
        public GameClientEncounterManager(GamePlayersRegistry playersRegistry)
        {
            _playersRegistry = playersRegistry;
        }
        
        public event Action? StateChanged;
        
        public event Action? AggressorChanged;
        
        public event Action? DefenderChanged;
        
        public event Action? PlanetChanged;

        IGamePlayer? IGameClientEncounterManager.AggressorPlayer => 
            _aggressorPlayer;

        IGamePlayer? IGameClientEncounterManager.DefenderPlayer => 
            _defenderPlayer;

        int? IGameClientEncounterManager.PlanetIdToAttack => 
            _planetIdToAttack;

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

        void IGameClientEncounterEvents.SetPlanetIdToAttack(int planetId)
        {
            _planetIdToAttack = planetId;
            PlanetChanged?.Invoke();
        }

        public void UpdateState(EncounterStateData state)
        {
            _aggressorPlayer = state.HasAggressorPlayerId 
                ? _playersRegistry.GetPlayerById(state.AggressorPlayerId) 
                : null;
            
            _defenderPlayer = state.HasDefenderPlayerId
                ? _playersRegistry.GetPlayerById(state.DefenderPlayerId)
                : null;
            
            _planetIdToAttack = state.HasPlanetToAttack
                ? state.PlanetIdToAttack
                : null;
            
            StateChanged?.Invoke();
        }
    }
}