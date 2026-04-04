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
        private readonly IGalaxyManagerNetwork _galaxyManager;
        private readonly GamePlayersRegistry _registry;
        private readonly GamePlayersPhaseTracker _playersPhaseTracker;
        
        public GameServerCoreLoader(
            IGameCardsManager cardsManager,
            IGalaxyManagerNetwork galaxyManager,
            GamePlayersRegistry registry,
            GamePlayersPhaseTracker playersPhaseTracker)
        {
            _cardsManager = cardsManager;
            _galaxyManager = galaxyManager;
            _registry = registry;
            _playersPhaseTracker = playersPhaseTracker;
        }

        public void Init()
        {
            // Сначала надо инициализировать карты
            _cardsManager.Init();
            _galaxyManager.Init();
            
            // Затем создаем руки
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