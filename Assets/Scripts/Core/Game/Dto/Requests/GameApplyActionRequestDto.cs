using System;
using Core.Game.Actions;

namespace Core.Game.Dto
{
    [Serializable]
    public class GameApplyActionRequestDto
    {
        public IGameAction Action = null!;
    }
}