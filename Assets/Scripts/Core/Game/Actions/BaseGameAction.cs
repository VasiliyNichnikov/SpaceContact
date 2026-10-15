using System;
using Core.Game.Validation;
using Newtonsoft.Json;

namespace Core.Game.Actions
{
    [Serializable]
    public abstract class BaseGameAction : IGameAction
    {
        [JsonProperty]
        [StateRequired]
        public Guid Id { get; private set; } = Guid.NewGuid();

        public abstract GameActionResult Execute(GameContext context, GameActor actor);
    }
}