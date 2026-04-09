using System;
using Core.Game.Cards;
using Core.Game.Dto.States.Cards;

namespace Core.Game.Phases.Client
{
    public interface IGameClientDestinyPhaseResolver
    {
        event Action? Changed;
        
        IDestinyCard? Card { get; }
        
        void UpdateState(DestinyCardStateData state);
    }
}