using System.Collections.Generic;
using System.Linq;
using Core.Game.Encounter;
using Core.Game.Mutation;
using Core.Game.Mutation.Events;
using Core.Game.Players;
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
        
        private GameEventRpcRelayNetwork? _relay;
        
        private GameServerEventBroadcaster(
            GamePlayersRegistry playersRegistry,
            GameServerSimpleEncounterState simpleEncounterState)
        {
            _playersRegistry = playersRegistry;
            _simpleEncounterState = simpleEncounterState;
        }        

        public void Bind(GameEventRpcRelayNetwork relay)
        {
            _relay = relay;
        }
        
        public void SendEvent(IServerGameEvent serverEvent, RecipientType recipientType)
        {
            if (_relay == null)
            {
                Logger.Error("ServerEventBroadcaster.Broadcast: relay is null.");
                return;
            }
            
            var events = CreateEventsData(serverEvent);
            var targets = GetClientTargetsByType(recipientType);
            
            _relay.SendEventsToClients(events, targets);
        }

        public void SendEvent(IEnumerable<IServerGameEvent> serverEvents, RecipientType recipientType)
        {
            if (_relay == null)
            {
                Logger.Error("ServerEventBroadcaster.Broadcast: relay is null.");
                return;
            }
            
            var events = CreateEventsData(serverEvents);
            var targets = GetClientTargetsByType(recipientType);
            
            _relay.SendEventsToClients(events, targets);
        }

        private GameEventsToClientsData CreateEventsData(IServerGameEvent gameEvent)
        {
            var gameEventStateData = gameEvent.ToState(this);

            return new GameEventsToClientsData
            {
                GameEvents = new[]
                {
                    gameEventStateData
                }
            };
        }

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

        GameEventStateData IGameEventToStateMapper<GameEventStateData>.Visit(GameServerAggressorSelectedEvent serverEvent)
        {
            var state = new GameEventStateData
            {
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
                DefenderSelectedEvent = new GameDefenderSelectedEventStateData
                {
                    DefenderPlayerId = serverEvent.DefenderPlayerId,
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