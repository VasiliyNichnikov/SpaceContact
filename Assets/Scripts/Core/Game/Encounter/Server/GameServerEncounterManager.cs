using System;
using System.Linq;
using Core.Game.Dto.States;
using Core.Game.Mutation;
using Core.Game.Players;
using Core.Game.Rules;
using Logs;

namespace Core.Game.Encounter
{
    public sealed class GameServerEncounterManager : IGameServerEncounterManager
    {
        private readonly IServerEventBroadcaster _broadcaster;
        private readonly GamePlayersRegistry _playersRegistry;
        private readonly GameRulesChecker _rulesChecker;
        private readonly GameServerEventsFactory _serverEventsFactory;
        private readonly GameServerSimpleEncounterState _simpleEncounterState;
        
        private int? _currentAggressorIndex;
        
        public GameServerEncounterManager(
            IServerEventBroadcaster broadcaster,
            GamePlayersRegistry playersRegistry,
            GameRulesChecker rulesChecker,
            GameServerEventsFactory serverEventsFactory,
            GameServerSimpleEncounterState simpleEncounterState)
        {
            _broadcaster = broadcaster;
            _playersRegistry = playersRegistry;
            _rulesChecker = rulesChecker;
            _serverEventsFactory = serverEventsFactory;
            _simpleEncounterState = simpleEncounterState;
        }
        
        public event Action? Started;

        public ulong? AggressorPlayerId => 
            _simpleEncounterState.AggressorPlayerId;
        
        public ulong? DefenderPlayerId => 
            _simpleEncounterState.DefenderPlayerId;
        
        public void StartEncounter()
        {
            var sortedByOrderPlayers = _playersRegistry.SortedByOrderPlayers.ToList();

            if (sortedByOrderPlayers.Count == 0)
            {
                Logger.Error($"{nameof(GameServerEncounterManager)}.{nameof(StartEncounter)}: no players found.");
                return;
            }
            
            if (_currentAggressorIndex == null)
            {
                _currentAggressorIndex = 0;
            }
            else
            {
                _currentAggressorIndex = _currentAggressorIndex.Value + 1 < sortedByOrderPlayers.Count 
                    ? _currentAggressorIndex.Value + 1 
                    : 0;
            }
            
            var selectedAggressorPlayerId = sortedByOrderPlayers[_currentAggressorIndex.Value].PlayerId;

            if (!_rulesChecker.Check(
                    GameRuleType.CanBeAggressor,
                    GameRuleContext.CheckPlayer(selectedAggressorPlayerId)))
            {
                
                Logger.Error($"{nameof(GameServerEncounterManager)}.{nameof(StartEncounter)}: it is impossible to expose the aggressor.");
                return;
            }
            
            _simpleEncounterState.SetAggressorPlayerId(selectedAggressorPlayerId);
            
            Started?.Invoke();
        }
        
        public void SetDefenderPlayerId(ulong playerId)
        {
            if (!_rulesChecker.Check(
                    GameRuleType.CanBeDefender,
                    GameRuleContext.CheckPlayer(playerId)))
            {
                
                Logger.Error($"{nameof(GameServerEncounterManager)}.{nameof(SetDefenderPlayerId)}: it is impossible to expose the defender.");
                return;
            }
         
            _simpleEncounterState.SetDefenderPlayerId(playerId);
            var selectedDefenderEvent = _serverEventsFactory.CreateDefenderSelectedEvent(playerId);
            _broadcaster.SendEvent(selectedDefenderEvent, RecipientType.AllClients);
        }

        public EncounterStateData ToState()
        {
            var state = new EncounterStateData();
            
            if (AggressorPlayerId != null)
            {
                state.HasAggressorPlayerId = true;
                state.AggressorPlayerId = AggressorPlayerId.Value;
            }

            if (DefenderPlayerId != null)
            {
                state.HasDefenderPlayerId = true;
                state.DefenderPlayerId = DefenderPlayerId.Value;
            }
            
            state.HasPlanetToAttack = false;
            Logger.Warning("GameServerEncounterManager.ToState: state changed.");

            return state;
        }
    }
}