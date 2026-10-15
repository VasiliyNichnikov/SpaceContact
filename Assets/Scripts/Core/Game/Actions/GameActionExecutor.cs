using System.Collections.Generic;
using System.Linq;
using System;
using Core.Game.Events;

namespace Core.Game.Actions
{
    public sealed class GameActionExecutor
    {
        private readonly GameContext _context;
        private readonly GameEventBatchFactory _batchFactory;
        private readonly Dictionary<GameActionKey, GameActionExecutionResult> _acceptedActions = new();

        public GameActionExecutor(GameContext context, GameEventBatchFactory batchFactory)
        {
            _context = context;
            _batchFactory = batchFactory;
        }

        public GameActionExecutionResult Execute(IGameAction action, GameActor actor)
        {
            _context.BeginServerOperation();

            try
            {
                if (!actor.IsSystem && (!actor.PlayerId.HasValue || !_context.Players.IsPlayerExist(actor.PlayerId.Value)))
                {
                    return GameActionExecutionResult.Rejected(_context.Revision, "The sender must be a game participant.");
                }

                if (action.Id == Guid.Empty)
                {
                    return GameActionExecutionResult.Rejected(_context.Revision, "The action ID must not be empty.");
                }

                var key = new GameActionKey(actor, action.Id);
                if (_acceptedActions.TryGetValue(key, out var accepted))
                {
                    return accepted.AsDuplicate();
                }

                var result = action.Execute(_context, actor);
                if (!result.IsSuccess)
                {
                    return GameActionExecutionResult.Rejected(_context.Revision, result.ErrorMessage);
                }

                // Own the event sequence before invoking events or synchronous observers.
                var events = result.Events.ToArray();
                var hasChanges = events.Length > 0;
                if (hasChanges && _context.Revision == long.MaxValue)
                {
                    return GameActionExecutionResult.Rejected(_context.Revision, "The game revision limit has been reached.");
                }

                var revision = hasChanges ? checked(_context.Revision + 1) : _context.Revision;
                foreach (var gameEvent in events)
                {
                    gameEvent.Apply(_context);
                }

                // Finish every recipient's payload before publishing the operation as committed.
                var batches = _batchFactory.CreateBatches(_context, revision, events);
                accepted = GameActionExecutionResult.Accepted(revision, hasChanges, batches);
                _acceptedActions.Add(key, accepted);
                if (hasChanges)
                {
                    _context.CommitServerOperation(revision);
                }

                return accepted;
            }
            catch
            {
                // Events may already have changed state. Never continue from a partial operation.
                _context.FaultServerOperation();
                throw;
            }
            finally
            {
                _context.EndServerOperation();
            }
        }
    }
}
