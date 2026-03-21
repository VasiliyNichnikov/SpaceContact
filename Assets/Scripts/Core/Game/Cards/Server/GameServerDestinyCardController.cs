using System;
using Core.Game.Dto.States.Cards;
using Logs;

namespace Core.Game.Cards
{
    public sealed class GameServerDestinyCardController : IGameServerDestinyCardController
    {
        private readonly IGameCardsManager _cardsManager;
        private DestinyCardStateData? _destinyCardState;
        
        public GameServerDestinyCardController(IGameCardsManager cardsManager)
        {
            _cardsManager = cardsManager;
        }

        public event Action? CardChanged;

        public void Init()
        {
            _destinyCardState = _cardsManager.OpenNextDestinyCard();
            CardChanged?.Invoke();
        }

        public DestinyCardStateData GetState()
        {
            if (_destinyCardState == null)
            {
                Logger.Error("GameServerDestinyCardControllerNetwork.GetState: destinyCardState is null.");

                return default;
            }
            
            return _destinyCardState.Value;
        }
    }
}