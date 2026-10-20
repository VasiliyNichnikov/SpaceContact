using System;
using System.Collections.Generic;
using Core.Game.Events;

namespace Core.Game.Actions
{
    public interface IGameAction
    {
        Guid Id { get; }
        
        IReadOnlyList<IGameEvent> Execute(GameContext context, GameActor actor);

        IActionValidator GetValidator();
    }
}