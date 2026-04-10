using Core.Game.Cards;
using Core.Game.Galaxy.Server;
using Core.Game.Phases;
using Core.Game.Players;

namespace Core.Game
{
    /// <summary>
    /// Инициализация серверных данных
    /// </summary>
    public class GameServerCoreLoader
    {
        private readonly IGameServerCardsManager _cardsManager;
        private readonly IGameServerGalaxyManager _galaxyManager;
        private readonly GamePlayersRegistry _registry;
        private readonly GamePlayersPhaseTracker _playersPhaseTracker;
        
        public GameServerCoreLoader(
            IGameServerCardsManager cardsManager,
            IGameServerGalaxyManager galaxyManager,
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
            _playersPhaseTracker.Init(_registry.Players);
        }
    }
}