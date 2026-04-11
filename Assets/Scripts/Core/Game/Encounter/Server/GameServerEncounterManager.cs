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
        private readonly GameServerSimpleEncounterState _encounterState;
        
        private int? _currentAggressorIndex;
        
        public GameServerEncounterManager(
            IServerEventBroadcaster broadcaster,
            GamePlayersRegistry playersRegistry,
            GameRulesChecker rulesChecker,
            GameServerEventsFactory serverEventsFactory,
            GameServerSimpleEncounterState encounterState)
        {
            _broadcaster = broadcaster;
            _playersRegistry = playersRegistry;
            _rulesChecker = rulesChecker;
            _serverEventsFactory = serverEventsFactory;
            _encounterState = encounterState;
        }

        private ulong? AggressorPlayerId => 
            _encounterState.AggressorPlayerId;
        
        private ulong? DefenderPlayerId => 
            _encounterState.DefenderPlayerId;

        private int? SelectedPlanetIdToAttack => 
            _encounterState.PlanetIdToAttack;
        
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
            
            _encounterState.SetAggressorPlayerId(selectedAggressorPlayerId);
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
         
            _encounterState.SetDefenderPlayerId(playerId);
            var selectedDefenderEvent = _serverEventsFactory.CreateDefenderSelectedEvent(playerId);
            _broadcaster.SendEvent(selectedDefenderEvent, RecipientType.AllClients);
        }

        public bool SetPlanetToAttack(ulong initiatedByPlayerId, int planetId)
        {
            return DefenderPlayerId == null 
                ? SetDefenderAndPlanetToAttack(initiatedByPlayerId, planetId) 
                : SetOnlyPlanetToAttack(initiatedByPlayerId, planetId);
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

            if (SelectedPlanetIdToAttack != null)
            {
                state.HasPlanetToAttack = true;
                state.PlanetIdToAttack = SelectedPlanetIdToAttack.Value;
            }
            
            Logger.Warning("GameServerEncounterManager.ToState: state changed.");

            return state;
        }

        private bool SetDefenderAndPlanetToAttack(ulong initiatedByPlayerId, int planetId)
        {
            var defenderPlayer = _playersRegistry.Players.FirstOrDefault(player => player.ContainsPlanet(planetId));

            if (defenderPlayer == null)
            {
                Logger.Error($"{nameof(GameServerEncounterManager)}.{nameof(SetDefenderAndPlanetToAttack)}: player with planet {planetId} not found.");
                
                return false;
            }
            
            if (!_rulesChecker.Check(
                    GameRuleType.CanBeDefender,
                    GameRuleContext.CheckPlayer(defenderPlayer.PlayerId)))
            {
                Logger.Error($"{nameof(GameServerEncounterManager)}.{nameof(SetDefenderAndPlanetToAttack)}: it is impossible to expose the defender.");
                
                return false;
            }
            
            if (!_rulesChecker.Check(
                    GameRuleType.CanChoosePlanetToAttack,
                    GameRuleContext.CheckPlanetToAttack(initiatedByPlayerId, planetId)))
            {
                Logger.Error($"{nameof(GameServerEncounterManager)}.{nameof(SetDefenderAndPlanetToAttack)}: it is impossible to choose a planet to attack.");
                
                return false;
            }
            
            _encounterState.SetDefenderPlayerId(defenderPlayer.PlayerId);
            _encounterState.SetPlanetIdToAttack(planetId);
            
            var planetIdToAttackSelectedEvent = _serverEventsFactory.CreatePlanetIdToAttackSelectedEvent(initiatedByPlayerId, planetId);
            var selectedDefenderEvent = _serverEventsFactory.CreateDefenderSelectedEvent(defenderPlayer.PlayerId);
            
            var events = new IServerGameEvent[]
            {
                planetIdToAttackSelectedEvent,
                selectedDefenderEvent
            };
            
            _broadcaster.SendEvent(events, RecipientType.AllClients);
            return true;
        }
        
        private bool SetOnlyPlanetToAttack(ulong initiatedByPlayerId, int planetId)
        {
            if (!_rulesChecker.Check(
                    GameRuleType.CanChoosePlanetToAttack,
                    GameRuleContext.CheckPlanetToAttack(initiatedByPlayerId, planetId)))
            {
                _encounterState.SetDefenderPlayerId(null);
                Logger.Error($"{nameof(GameServerEncounterManager)}.{nameof(SetOnlyPlanetToAttack)}: it is impossible to choose a planet to attack.");
                
                return false;
            }
            
            _encounterState.SetPlanetIdToAttack(planetId);
            var planetIdToAttackSelectedEvent = _serverEventsFactory.CreatePlanetIdToAttackSelectedEvent(initiatedByPlayerId, planetId);
            _broadcaster.SendEvent(planetIdToAttackSelectedEvent, RecipientType.AllClients);

            return true;
        }
    }
}