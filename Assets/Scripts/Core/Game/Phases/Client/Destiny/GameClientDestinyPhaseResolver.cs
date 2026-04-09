using System;
using Core.Game.Cards;
using Core.Game.Dto.States.Cards;

namespace Core.Game.Phases.Client
{
    public sealed class GameClientDestinyPhaseResolver : IGameClientDestinyPhaseResolver
    {
        private readonly DestinyCardFactory _destinyCardFactory;
        
        public GameClientDestinyPhaseResolver(DestinyCardFactory destinyCardFactory)
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