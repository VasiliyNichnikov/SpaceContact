using System;
using Newtonsoft.Json;

namespace Core.Game.Actions
{
    [Serializable]
    public abstract class BaseGameAction : IGameAction
    {
        [JsonProperty]
        public Guid Id { get; private set; } = Guid.NewGuid();

        public abstract GameActionResult Execute(GameContext context, GameActor actor);
    }
}