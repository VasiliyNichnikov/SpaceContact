using System;
using Core.Game.Cards;
using Core.Game.Dto.States.Cards;
using Core.Game.Encounter;
using Logs;

namespace Core.Game.Phases.Server
{
    public class GameServerDestinyPhaseResolver : IGameServerDestinyPhaseResolver
    {
        private readonly IGameCardsManager _cardsManager;
        private readonly IGameServerEncounterManager _encounterManager;
        private readonly GameDestinyTargetSelector _targetSelector;
        
        private DestinyCardStateData? _currentDestinyCardState;
        
        public GameServerDestinyPhaseResolver(
            IGameServerEncounterManager encounterManager, 
            IGameCardsManager cardsManager,
            GameDestinyTargetSelector targetSelector)
        {
            _encounterManager = encounterManager;
            _cardsManager = cardsManager;
            _targetSelector = targetSelector;
        }
        
        public event Action? Changed;

        public void ChooseDestiny()
        {
            _currentDestinyCardState = _cardsManager.OpenNextDestinyCard();
            TrySetDefenderPlayerImmediately(_currentDestinyCardState.Value);
            Changed?.Invoke();
        }

        public DestinyCardStateData ToState()
        {
            if (_currentDestinyCardState == null)
            {
                Logger.Error($"{nameof(GameServerDestinyPhaseResolver)}.{nameof(ToState)}: No current Destiny Card State.");
                return default;
            }

            return _currentDestinyCardState.Value;
        }

        private void TrySetDefenderPlayerImmediately(DestinyCardStateData card)
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