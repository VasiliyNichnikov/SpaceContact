using Core.Game.Cards;
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
        
        public GameServerCoreLoader(
            IGameCardsManager cardsManager,
            GamePlayersRegistry registry)
        {
            _cardsManager = cardsManager;
            _registry = registry;
        }

        public void Init()
        {
            foreach (var player in _registry.Players)
            {
                var handState = _cardsManager.CreatePlayerHand();
                var handDistributionVisitor = new GamePlayerHandDistributionVisitor(handState);
                player.Apply(handDistributionVisitor);
            }
        }
    }
}