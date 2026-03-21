using Core.Game.Cards;
using Core.Game.Phases;
using Core.Game.Players;
using Core.Game.Players.Visitors;

namespace Core.Game
{
    /// <summary>
    /// Инициализация серверных данных
    /// </summary>
    public class GameServerCoreLoader
    {
        private readonly IGameCardsManager _cardsManager;
        private readonly GamePlayersRegistry _registry;
        private readonly GamePlayersPhaseTracker _playersPhaseTracker;
        
        public GameServerCoreLoader(
            IGameCardsManager cardsManager,
            GamePlayersRegistry registry,
            GamePlayersPhaseTracker playersPhaseTracker)
        {
            _cardsManager = cardsManager;
            _registry = registry;
            _playersPhaseTracker = playersPhaseTracker;
        }

        public void Init()
        {
            foreach (var player in _registry.Players)
            {
                var handState = _cardsManager.CreatePlayerHand();
                var handDistributionVisitor = new GamePlayerHandDistributionVisitor(handState);
                player.Apply(handDistributionVisitor);
            }
            
            _playersPhaseTracker.Init(_registry.Players);
        }
    }
}