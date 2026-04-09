using Core.Game.Cards;
using Core.Game.Dto.States.Cards;
using Core.Game.Encounter;
using Core.Game.Mutation;

namespace Core.Game.Phases.Server
{
    public class GameServerDestinyPhaseResolver : IGameServerDestinyPhaseResolver
    {
        private readonly IGameCardsManager _cardsManager;
        private readonly IGameServerEncounterManager _encounterManager;
        private readonly GameDestinyTargetSelector _targetSelector;
        private readonly IServerEventBroadcaster _broadcaster;
        private readonly GameServerEventsFactory _eventsFactory;
        
        public GameServerDestinyPhaseResolver(
            IGameServerEncounterManager encounterManager, 
            IGameCardsManager cardsManager,
            GameDestinyTargetSelector targetSelector,
            IServerEventBroadcaster broadcaster,
            GameServerEventsFactory eventsFactory)
        {
            _encounterManager = encounterManager;
            _cardsManager = cardsManager;
            _targetSelector = targetSelector;
            _broadcaster = broadcaster;
            _eventsFactory = eventsFactory;
        }
        
        public void ChooseDestiny()
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