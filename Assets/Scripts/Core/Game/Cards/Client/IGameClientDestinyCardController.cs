using System;
using Core.Game.Dto.States.Cards;

namespace Core.Game.Cards
{
    public interface IGameClientDestinyCardController
    {
        event Action? Changed;
        
        IDestinyCard? Card { get; }
        
        void UpdateState(DestinyCardStateData state);
    }
}