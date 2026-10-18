using System;
using System.Collections.Generic;
using System.Linq;
using Logs;

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

        internal bool TryAdd(IGameAction action)
        {
            if (!_actionsById.TryAdd(action.Id, action))
            {
                return false;
            }

            NotifyChanged();
            return true;
        }

        internal void Remove(Guid actionId)
        {
            if (_actionsById.Remove(actionId))
            {
                NotifyChanged();
            }
        }

        private void NotifyChanged()
        {
            if (Changed is not { } handlers)
            {
                return;
            }

            foreach (var action in handlers.GetInvocationList())
            {
                var handler = (Action)action;
                try
                {
                    handler();
                }
                catch (Exception exception)
                {
                    Logger.Error($"Pending action observer failed: {exception}");
                }
            }
        }
    }
}
