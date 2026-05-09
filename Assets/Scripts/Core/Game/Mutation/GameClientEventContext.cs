using Core.Game.Dto;
using Core.Game.Encounter;
using Core.Game.Phases.Client;
using Core.Game.Players;
using Core.Game.Rules;
using Logs;
using Network.Game.Mutation;

namespace Core.Game.Mutation
{
    public sealed class GameClientEventContext : IGameEventContext
    {
        private readonly GameRulesChecker _rulesChecker;
        private readonly GamePlayersRegistry _playersRegistry;
        private readonly IGameClientEncounterEvents _encounterEvents;
        private readonly IGameClientDestinyPhaseResolver _destinyPhaseResolver;
        private readonly IGameClientPlayerReadinessEvents _readinessEvents;
        
        private int _lastEventId = int.MinValue;

        public GameClientEventContext(
            GameRulesChecker rulesChecker,
            GamePlayersRegistry playersRegistry,
            IGameClientEncounterEvents encounterEvents,
            IGameClientDestinyPhaseResolver destinyPhaseResolver,
            IGameClientPlayerReadinessEvents readinessEvents)
        {
            _rulesChecker = rulesChecker;
            _playersRegistry = playersRegistry;
            _encounterEvents = encounterEvents;
            _destinyPhaseResolver = destinyPhaseResolver;
            _readinessEvents = readinessEvents;
        }
        
        public void Execute(GameAggressorSelectedEventData evt)
        {
            if (!CheckLastEventId(evt))
            {
                return;
            }
            
            var context = GameRuleContext.CheckPlayer(evt.AggressorPlayerId);
            
            if (!_rulesChecker.Check(GameRuleType.CanBeAggressor, context))
            {
                Logger.Error($"{nameof(GameClientEventContext)}.{nameof(Execute)}: it is impossible to choose an aggressor.");
                return;
            }
            
            _encounterEvents.SetAggressorEvent(evt.AggressorPlayerId);
        }

        public void Execute(GameDefenderSelectedEventData evt)
        {
            if (!CheckLastEventId(evt))
            {
                return;
            }
            
            var context = GameRuleContext.CheckPlayer(evt.DefenderPlayerId);
            
            if (!_rulesChecker.Check(GameRuleType.CanBeDefender, context))
            {
                Logger.Error($"{nameof(GameClientEventContext)}.{nameof(Execute)}: it is impossible to choose an defender.");
                return;
            }
            
            _encounterEvents.SetDefenderEvent(evt.DefenderPlayerId);
        }

        public void Execute(GameDestinyCardChangedEventData evt)
        {
            if (!CheckLastEventId(evt))
            {
                return;
            }
            
            if (!_rulesChecker.Check(GameRuleType.CanApplyDestinyCard, GameRuleContext.Empty))
            {
                Logger.Error($"{nameof(GameClientEventContext)}.{nameof(Execute)}: it is impossible to apply the destiny card.");
                
                return;
            }
            
            _destinyPhaseResolver.UpdateState(evt.DestinyCard);
        }

        public void Execute(GamePlanetToAttackSelectedEventData evt)
        {
            if (!CheckLastEventId(evt))
            {
                return;
            }
            
            var context = GameRuleContext.CheckPlanetToAttack(evt.InitiatedByPlayerId, evt.PlanetId);
            
            if (!_rulesChecker.Check(GameRuleType.CanChoosePlanetToAttack, context))
            {
                Logger.Error($"{nameof(GameClientEventContext)}.{nameof(Execute)}: it is impossible to choose a planet to attack.");
                
                return;
            }
            
            _encounterEvents.SetPlanetIdToAttack(evt.PlanetId);
        }

        public void Execute(GamePlayerReadinessEventData evt)
        {
            if (!CheckLastEventId(evt))
            {
                return;
            }

            var ownerPlayer = _playersRegistry.GetOwnerWithError();

            if (ownerPlayer == null || ownerPlayer.PlayerId != evt.SelectedPlayerId)
            {
                return;
            }

            var context = GameRuleContext.CheckPlayer(evt.SelectedPlayerId);

            switch (evt.IsPlayerReadyToNextPhase)
            {
                case true when _rulesChecker.Check(GameRuleType.CanPlayerChangeToReady, context):
                    ownerPlayer.IsReadyToNextPhase = true;
                    _readinessEvents.SetReady();
                    break;
                    
                case false when _rulesChecker.Check(GameRuleType.CanPlayerChangeToNotReady, context):
                    ownerPlayer.IsReadyToNextPhase = false;
                    _readinessEvents.SetNotReady();
                    break;
                    
                default:
                    Logger.Error($"{nameof(GameClientEventContext)}.{nameof(Execute)}: Failed to perform checks to switch the player's readiness.");
                    break;
            }
        }

        private bool CheckLastEventId(GameEventAbstractData data)
        {
            if (_lastEventId > data.Metadata.EventId)
            {
                Logger.Error($"{nameof(GameClientEventContext)}.{nameof(CheckLastEventId)}: {data.Metadata.EventId} the event id is larger than the last event.");
                return false;
            }

            _lastEventId = data.Metadata.EventId;

            return true;
        }
    }
}