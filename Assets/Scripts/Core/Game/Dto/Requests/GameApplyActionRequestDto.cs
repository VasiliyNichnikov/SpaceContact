using System;
using Core.Game.Actions;

namespace Core.Game.Dto
{
    [Serializable]
    public class GameApplyActionRequestDto
    {
        // Missing or null actions are rejected by the request handler.
        public ClientGameAction? Action;
    }
}
