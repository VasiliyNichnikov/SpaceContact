using System;
using System.Linq;
using Core.Game.Dto;
using Logs;

namespace Core.Game.Events
{
    public sealed class GameEventBatchApplier
    {
        private readonly ClientGameContext _context;

        public GameEventBatchApplier(ClientGameContext context)
        {
            _context = context;
        }

        public event Action<GameEventBatchApplyResult, long>? Completed;

        public GameEventBatchApplyResult Apply(GameEventBatchData? batch)
        {
            var result = ApplyInternal(batch);

            if (Completed != null)
            {
                foreach (var actionRaw in Completed.GetInvocationList())
                {
                    try
                    {
                        var action = (Action<GameEventBatchApplyResult, long>)actionRaw;
                        action.Invoke(result, batch?.Revision ?? _context.Revision);
                    }
                    catch (Exception exception)
                    {
                        Logger.Error($"Batch completion observer failed: {exception}");
                    }
                }
            }

            return result;
        }

        private static bool AreEventsValid(IGameEvent?[]? events)
        {
            return events != null && events.All(gameEvent => gameEvent != null);
        }

        private GameEventBatchApplyResult ApplyInternal(GameEventBatchData? batch)
        {
            _context.BeginClientBatch();
            var revision = batch?.Revision ?? _context.Revision;

            try
            {
                if (batch is null || revision < 0)
                {
                    return RequireSnapshot(_context.Revision, "The batch is missing or has an invalid revision.");
                }

                if (_context.IsSynchronizationRequired)
                {
                    return RequireSnapshot(revision, "A current snapshot is required before applying live events.");
                }

                if (revision <= _context.Revision)
                {
                    return new GameEventBatchApplyResult(GameEventBatchApplyStatus.Duplicate, _context.Revision);
                }

                // A larger revision exists here, so the current revision cannot be long.MaxValue.
                if (revision != _context.Revision + 1)
                {
                    return RequireSnapshot(revision, "A game revision was skipped.");
                }

                var events = batch.Events;
                
                if (!AreEventsValid(events))
                {
                    return RequireSnapshot(revision, "The event list is missing or contains a null event.");
                }

                foreach (var gameEvent in events)
                {
                    gameEvent.Apply(_context);
                }

                _context.CommitClientBatch(revision);
                return new GameEventBatchApplyResult(GameEventBatchApplyStatus.Applied, revision);
            }
            catch (Exception exception)
            {
                var result = RequireSnapshot(revision, "The batch could not be applied; restore the client from a snapshot.");
                Logger.Error($"Client batch application failed at revision {revision}: {exception}");
                return result;
            }
            finally
            {
                _context.EndClientBatch();
            }
        }

        private GameEventBatchApplyResult RequireSnapshot(long revision, string message)
        {
            _context.RequireClientSnapshot(revision);
            return new GameEventBatchApplyResult(GameEventBatchApplyStatus.SnapshotRequired, _context.Revision, message);
        }
    }
}
