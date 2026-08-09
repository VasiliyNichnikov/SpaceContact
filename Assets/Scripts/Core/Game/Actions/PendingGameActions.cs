using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Game.Actions
{
    public sealed class PendingGameActions
    {
        private readonly Dictionary<Guid, IGameAction> _actionsById = new();

        public event Action? Changed;

        public bool HasAny => 
            _actionsById.Count > 0;

        public bool Contains(Guid actionId) =>
            _actionsById.ContainsKey(actionId);

        public bool Contains<TAction>() where TAction : IGameAction
        {
            return _actionsById.Values.Any(action => action is TAction);
        }

        public void Add(IGameAction action)
        {
            _actionsById[action.Id] = action;
            Changed?.Invoke();
        }

        public void Complete(Guid actionId)
        {
            if (_actionsById.Remove(actionId))
            {
                Changed?.Invoke();
            }
        }

        public void Cancel(Guid actionId)
        {
            if (_actionsById.Remove(actionId))
            {
                Changed?.Invoke();
            }
        }
    }
}