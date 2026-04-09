using System;
using Core.Game.Dto.States.Cards;

namespace Core.Game.Phases.Server
{
    public interface IGameServerDestinyPhaseResolver
    {
        event Action? Changed;

        void ChooseDestiny();
        
        DestinyCardStateData ToState();
    }
}