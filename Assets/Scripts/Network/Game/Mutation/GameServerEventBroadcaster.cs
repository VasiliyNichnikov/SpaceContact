using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Game.Encounter;
using Core.Game.Mutation;
using Core.Game.Players;
using GeneralUtils;
using Network.Dto;
using Logger = Logs.Logger;

namespace Network.Game.Mutation
{
    public sealed class GameServerEventBroadcaster : IServerEventBroadcaster
    {
        private readonly GamePlayersRegistry _playersRegistry;
        private readonly GameServerSimpleEncounterState _simpleEncounterState;
        private readonly TaskCompletionSource<bool> _readyTcs = 
            new(TaskCreationOptions.RunContinuationsAsynchronously);
        
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
        
        void IServerEventBroadcaster.SendEvent(IGameEventData evt, RecipientType recipientType)
        {
            var evts = new List<IGameEventData>()
            {
                evt
            };
            
            SendEventInternal(evts, recipientType).FireAndForget();
        }

        void IServerEventBroadcaster.SendEvent(IEnumerable<IGameEventData> evts, RecipientType recipientType) => 
            SendEventInternal(evts, recipientType).FireAndForget();

        private static GameEventsToClientsData CreateEventsData(IEnumerable<IGameEventData> gameEvents)
        {
            return new GameEventsToClientsData
            {
                GameEvents = gameEvents.ToArray()
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
        
        private async Task SendEventInternal(IEnumerable<IGameEventData> evts, RecipientType recipientType)
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
            
            var events = CreateEventsData(evts);
            var targets = GetClientTargetsByType(recipientType);
            
            _relay.SendEventsToClients(events, targets);
        }
    }
}