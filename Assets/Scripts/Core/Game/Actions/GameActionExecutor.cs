using System;
using System.Collections.Generic;
using System.Linq;
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

        public GameActionExecutionResult ExecuteClientAction(ClientGameAction action, ulong senderId)
        {
            return Execute(action, GameActor.Player(senderId));
        }

        public GameActionExecutionResult ExecuteServerAction(ServerGameAction action)
        {
            return Execute(action, GameActor.System());
        }

        internal GameActionExecutionResult ExecuteAutomaticAction(AutomaticClientGameAction action)
        {
            return Execute(action, GameActor.System());
        }

        private GameActionExecutionResult Execute(BaseGameAction action, GameActor initiator)
        {
            _context.BeginServerOperation();

            try
            {
                if ((action is not ClientGameAction && action is not ServerGameAction) ||
                    (!initiator.IsSystem && action is not ClientGameAction))
                {
                    return GameActionExecutionResult.Rejected(_context.Revision, "The action category is not allowed for this initiator.");
                }

                if (initiator.IsSystem && action is ClientGameAction &&
                    action is not AutomaticClientGameAction)
                {
                    return GameActionExecutionResult.Rejected(_context.Revision, "This client action cannot run automatically.");
                }

                if (!initiator.IsSystem && (!initiator.PlayerId.HasValue || !_context.Players.IsPlayerExist(initiator.PlayerId.Value)))
                {
                    return GameActionExecutionResult.Rejected(_context.Revision, "The sender must be a game participant.");
                }

                if (action.Id == Guid.Empty)
                {
                    return GameActionExecutionResult.Rejected(_context.Revision, "The action ID must not be empty.");
                }

                var key = new GameActionKey(initiator, action.Id);
                if (_acceptedActions.TryGetValue(key, out var accepted))
                {
                    return accepted.AsDuplicate();
                }

                var actor = initiator;
                if (initiator.IsSystem && action is ClientGameAction)
                {
                    var aggressorPlayerId = _context.Encounter.AggressorPlayerId;
                    if (!aggressorPlayerId.HasValue || !_context.Players.IsPlayerExist(aggressorPlayerId.Value))
                    {
                        return GameActionExecutionResult.Rejected(_context.Revision, "Automatic actions require an existing aggressor.");
                    }

                    actor = GameActor.Player(aggressorPlayerId.Value);
                }

                var validation = action.GetValidator().Check(_context, actor);

                if (!validation.IsSuccess)
                {
                    return GameActionExecutionResult.Rejected(_context.Revision, validation.ErrorMessage, validation.Result);
                }

                // Own the event sequence before invoking events or synchronous observers.
                var events = action.Execute(_context, actor).ToArray();
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
