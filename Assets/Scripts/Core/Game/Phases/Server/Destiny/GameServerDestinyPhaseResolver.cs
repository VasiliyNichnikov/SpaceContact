using Core.Game.Cards;
using Core.Game.Dto.States.Cards;
using Core.Game.Encounter;
using Core.Game.Mutation;
using Core.Game.Rules;
using Logs;

namespace Core.Game.Phases.Server
{
    public class GameServerDestinyPhaseResolver : IGameServerDestinyPhaseResolver
    {
        private readonly IGameServerCardsManager _cardsManager;
        private readonly IGameServerEncounterManager _encounterManager;
        private readonly GameDestinyTargetSelector _targetSelector;
        private readonly IServerEventBroadcaster _broadcaster;
        private readonly GameServerEventsFactory _eventsFactory;
        private readonly GameRulesChecker _rulesChecker;
        
        public GameServerDestinyPhaseResolver(
            IGameServerEncounterManager encounterManager, 
            IGameServerCardsManager cardsManager,
            GameDestinyTargetSelector targetSelector,
            IServerEventBroadcaster broadcaster,
            GameServerEventsFactory eventsFactory,
            GameRulesChecker rulesChecker)
        {
            _encounterManager = encounterManager;
            _cardsManager = cardsManager;
            _targetSelector = targetSelector;
            _broadcaster = broadcaster;
            _eventsFactory = eventsFactory;
            _rulesChecker = rulesChecker;
        }
        
        public void ChooseDestiny()
        {
            if(!_rulesChecker.Check(GameRuleType.CanApplyDestinyCard, GameRuleContext.Empty))
            {
                Logger.Error($"{nameof(GameServerDestinyPhaseResolver)}.{nameof(ChooseDestiny)}: can't apply Destiny card.");
                return;
            }
            
            OpenNextDestinyCard();
        }

        public bool SkipDestiny(ulong senderId)
        {
            var context = GameRuleContext.CheckPlayer(senderId);
            
            if(!_rulesChecker.Check(GameRuleType.CanSkipDestinyCard, context))
            {
                Logger.Error($"{nameof(GameServerDestinyPhaseResolver)}.{nameof(ChooseDestiny)}: can't apply Destiny card.");
                return false;
            }

            OpenNextDestinyCard();
            
            return true;
        }

        private void OpenNextDestinyCard()
        {
            var destinyCardData = _cardsManager.OpenNextDestinyCard();
            SendDestinyCardDataToClients(destinyCardData);
            TrySetDefenderPlayerImmediately(destinyCardData);
        }

        private void SendDestinyCardDataToClients(DestinyCardData data)
        {
            var destinyCardChangedEvent = _eventsFactory.CreateDestinyCardChangedEvent(data);
            _broadcaster.SendEvent(destinyCardChangedEvent, RecipientType.AllClients);
        }

        private void TrySetDefenderPlayerImmediately(DestinyCardData card)
        {
            var defenderId = _targetSelector.GetTarget(card);

            if (defenderId == null)
            {
                return;
            }
            
            _encounterManager.SetDefenderPlayerId(defenderId.Value);
        }
    }
}