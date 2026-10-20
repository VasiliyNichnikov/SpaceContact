using System;
using System.Collections.Generic;
using Core.Game.Events;
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

        public IReadOnlyList<IGameEvent> Execute(GameContext context, GameActor actor)
        {
            context.EnsureActionExecutionAllowed();
            return ExecuteCore(context, actor);
        }

        public abstract IActionValidator GetValidator();

        protected abstract IReadOnlyList<IGameEvent> ExecuteCore(GameContext context, GameActor actor);
    }
}
