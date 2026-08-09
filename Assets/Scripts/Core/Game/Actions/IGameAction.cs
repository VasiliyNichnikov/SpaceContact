using System;

namespace Core.Game.Actions
{
    public interface IGameAction
    {
        Guid Id { get; }
        
        GameActionResult Execute(GameContext context, GameActor actor);
    }
}