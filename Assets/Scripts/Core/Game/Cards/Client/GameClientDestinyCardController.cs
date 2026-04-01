using System;
using Core.Game.Dto.States.Cards;

namespace Core.Game.Cards
{
    public sealed class GameClientDestinyCardController : IGameClientDestinyCardController
    {
        private readonly DestinyCardFactory _destinyCardFactory;
        
        public GameClientDestinyCardController(DestinyCardFactory destinyCardFactory)
        {
            _destinyCardFactory = destinyCardFactory;
        }
        
        public event Action? Changed;
        
        public IDestinyCard? Card { get; private set; }
        
        public void UpdateState(DestinyCardStateData state)
        {
            Card = _destinyCardFactory.Create(state);
            Changed?.Invoke();
        }
    }
}