using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Game.Encounter;
using Core.Game.Mutation;
using Core.Game.Mutation.Events;
using Core.Game.Players;
using GeneralUtils;
using Network.Dto;
using UnityEngine;
using Logger = Logs.Logger;

namespace Network.Game.Mutation
{
    public sealed class GameServerEventBroadcaster : IServerEventBroadcaster, 
        IGameEventToStateMapper<GameEventStateData>
    {
        private readonly GamePlayersRegistry _playersRegistry;
        private readonly GameServerSimpleEncounterState _simpleEncounterState;
        private readonly TaskCompletionSource<bool> _readyTcs = new();
        
        private GameEventRpcRelayNetwork? _relay;
        
        private GameServerEventBroadcaster(
            GamePlayersRegistry playersRegistry,
            GameServerSimpleEncounterState simpleEncounterState)
        {
            _playersRegistry = playersRegistry;
            _simpleEncounterState = simpleEncounterState;
        }        

        public void SetRelayObject(GameEventRpcRelayNetwork relay)
        {
            _relay = relay;
            _readyTcs.SetResult(true);
        }
        
        void IServerEventBroadcaster.SendEvent(IServerGameEvent serverEvent, RecipientType recipientType)
        {
            var serverEvents = new List<IServerGameEvent>()
            {
                serverEvent
            };
            
            SendEventInternal(serverEvents, recipientType).FireAndForget();
        }

        void IServerEventBroadcaster.SendEvent(IEnumerable<IServerGameEvent> serverEvents, RecipientType recipientType) => 
            SendEventInternal(serverEvents, recipientType).FireAndForget();

        private GameEventsToClientsData CreateEventsData(IEnumerable<IServerGameEvent> gameEvents)
        {
            var gameEventStatesData = gameEvents.Select(g => g.ToState(this)).ToArray();
            return new GameEventsToClientsData
            {
                GameEvents = gameEventStatesData
            };
        }

        private List<ulong> GetClientTargetsByType(RecipientType recipientType)
        {
            var result = new List<ulong>();
            
            switch (recipientType)
            {
                case RecipientType.AllClients:
                    var allPlayers = _playersRegistry.Players.Select(p => p.PlayerId);
                    result.AddRange(allPlayers);
                    break;
                
                case RecipientType.AggressorClient:
                    if (_simpleEncounterState.AggressorPlayerId != null)
                    {
                        result.Add(_simpleEncounterState.AggressorPlayerId.Value);
                    }
                    break;
                    
                case RecipientType.DefenderClient:
                    if (_simpleEncounterState.DefenderPlayerId != null)
                    {
                        result.Add(_simpleEncounterState.DefenderPlayerId.Value);
                    }
                    break;
                
                default:
                    Logger.Error($"ServerEventBroadcaster.GetClientTargetsByType: recipientType is not supported: {recipientType}.");
                    break;
            }

            return result;
        }
        
        private async Task SendEventInternal(IEnumerable<IServerGameEvent> serverEvents, RecipientType recipientType)
        {
            if (_relay == null)
            {
                await _readyTcs.Task;
            }

            if (_relay == null)
            {
                Logger.Error($"{nameof(GameServerEventBroadcaster)}.{nameof(SendEventInternal)} relay is null.");
                
                return;
            }
            
            var events = CreateEventsData(serverEvents);
            var targets = GetClientTargetsByType(recipientType);
            
            _relay.SendEventsToClients(events, targets);
        }

        GameEventStateData IGameEventToStateMapper<GameEventStateData>.Visit(GameServerAggressorSelectedEvent serverEvent)
        {
            var state = new GameEventStateData
            {
                HasAggressorSelectedEvent = true,
                AggressorSelectedEvent = new GameAggressorSelectedEventStateData
                {
                    AggressorPlayerId = serverEvent.AggressorPlayerId,
                    Metadata = CreateGameEventMetadata(serverEvent)
                }
            };
            
            return state;
        }

        GameEventStateData IGameEventToStateMapper<GameEventStateData>.Visit(GameServerDefenderSelectedEvent serverEvent)
        {
            var state = new GameEventStateData
            {
                HasDefenderSelectedEvent = true,
                DefenderSelectedEvent = new GameDefenderSelectedEventStateData
                {
                    DefenderPlayerId = serverEvent.DefenderPlayerId,
                    Metadata = CreateGameEventMetadata(serverEvent)
                }
            };

            return state;
        }

        GameEventStateData IGameEventToStateMapper<GameEventStateData>.Visit(GameServerDestinyCardChangedEvent serverEvent)
        {
            var state = new GameEventStateData
            {
                HasDestinyCardChangedEvent = true,
                DestinyCardChangedEvent = new GameDestinyCardChangedEventStateData
                {
                    DestinyCard = serverEvent.DestinyCardData,
                    Metadata = CreateGameEventMetadata(serverEvent)
                }
            };

            return state;
        }

        GameEventStateData IGameEventToStateMapper<GameEventStateData>.Visit(GameServerPlanetToAttackSelectedEvent serverEvent)
        {
            var state = new GameEventStateData
            {
                HasPlanetToAttackSelectedEvent = true,
                PlanetSelectedEvent = new GamePlanetToAttackSelectedEventStateData
                {
                    PlanetId = serverEvent.PlanetId,
                    InitiatedByPlayerId = serverEvent.InitiatedByPlayerId,
                    Metadata = CreateGameEventMetadata(serverEvent)
                }
            };

            return state;
        }

        private static GameEventMetadata CreateGameEventMetadata(IServerGameEvent gameEvent)
        {
            var createdAtSeconds = Mathf.FloorToInt(Time.realtimeSinceStartup);
            
            return new GameEventMetadata
            {
                EventId = gameEvent.EventId,
                CreatedAtSeconds = createdAtSeconds
            };
        }
    }
}