using System;
using Core.Game.Dto.States.Cards;

namespace Core.Game.Cards
{
    public interface IGameServerDestinyCardController
    {
        event Action? CardChanged;
        
        DestinyCardStateData GetState();

        void Init();
    }
}