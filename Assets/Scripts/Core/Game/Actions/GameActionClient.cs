using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Game.Dto;
using Core.Game.Events;
using GeneralUtils;
using Logs;

namespace Core.Game.Actions
{
    public sealed class GameActionClient : IDisposable
    {
        private readonly IGameActionServerInteraction _network;
        private readonly GameContext _context;
        private readonly PendingGameActions _pendingActions;
        private readonly GameEventBatchApplier _batchApplier;
        private readonly CancellationTokenSource _lifetimeCancellation = new();
        private bool _disposed;

        public GameActionClient(IGameActionServerInteraction network, GameContext context,
            PendingGameActions pendingActions, GameEventBatchApplier batchApplier)
        {
            _network = network;
            _context = context;
            _pendingActions = pendingActions;
            _batchApplier = batchApplier;
        }

        public event Action<GameActionSendResult>? Completed;

        public bool ValidateAction(ClientGameAction action)
        {
            var localPlayer = _context.LocalPlayerState;

            if (localPlayer == null || _pendingActions.HasAny)
            {
                return false;
            }

            var validator = action.GetValidator();

            return validator.Check(_context, GameActor.Player(localPlayer.Id)).IsSuccess;
        }
        
        public void Send(ClientGameAction action)
        {
            SendAsync(action, CancellationToken.None).FireAndForget(exception => Logger.Error($"Action send failed: {exception}"));
        }

        public void Dispose()
        {
            _disposed = true;
            _lifetimeCancellation.Cancel();
            _lifetimeCancellation.Dispose();
        }

        private static async Task ObserveAbandonedRequestAsync(Task request)
        {
            try
            {
                await request;
            }
            catch (Exception exception)
            {
                Logger.Error($"Action request failed after waiting ended: {exception}");
            }
        }

        private async Task<GameActionSendResult> SendAsync(ClientGameAction action, CancellationToken ct)
        {
            var actionId = action.Id;
            if (_disposed || ct.IsCancellationRequested)
            {
                return CreateCancelledResult(actionId, false);
            }

            EnsureCanSend();
            if (actionId == Guid.Empty)
            {
                throw new ArgumentException("An action must have a non-empty ID.", nameof(action));
            }

            // Acquire cancellation before TryAdd invokes observers that can dispose the scene scope.
            using var cancellation = CancellationTokenSource.CreateLinkedTokenSource(ct, _lifetimeCancellation.Token);
            var token = cancellation.Token;
            var cancelled = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            await using var registration = token.Register(() => cancelled.TrySetResult(true));
            if (!_pendingActions.TryAdd(action))
            {
                throw new InvalidOperationException($"Action {actionId} is already pending.");
            }

            GameActionSendResult? result = null;
            var wasSent = false;
            try
            {
                if (token.IsCancellationRequested)
                {
                    result = CreateCancelledResult(actionId, false);
                    return result;
                }

                // Pending.Changed may have changed context availability before sending.
                EnsureCanSend();
                wasSent = true;
                var request = _network.SendAsync(action, GameActor.Player(_context.LocalPlayerId), token);
                await Task.WhenAny(request, cancelled.Task);
                if (token.IsCancellationRequested)
                {
                    ObserveAbandonedRequestAsync(request).FireAndForget();
                    result = CreateCancelledResult(actionId, true);
                    return result;
                }

                var response = await request;
                result = ProcessResponse(actionId, response);
                return result;
            }
            catch (Exception exception)
            {
                if (token.IsCancellationRequested)
                {
                    result = CreateCancelledResult(actionId, wasSent);
                    return result;
                }

                if (wasSent)
                {
                    result = RequireSnapshot(actionId, exception.Message);
                }

                throw;
            }
            finally
            {
                _pendingActions.Remove(actionId);
                // Removing pending invokes observers too; shutdown suppresses completion notifications.
                if (!_disposed && result != null)
                {
                    NotifyCompleted(result);
                }
            }
        }

        private void EnsureCanSend()
        {
            if (_context.Role != GameContextRole.Client)
            {
                throw new InvalidOperationException("The action client requires a client context.");
            }

            if (_context.IsBusy || _context.IsSynchronizationRequired || _context.LocalPlayerState is null)
            {
                throw new InvalidOperationException("A synchronized local player is required to send an action.");
            }
        }

        private GameActionSendResult ProcessResponse(Guid actionId, GameApplyActionResponseDto? response)
        {
            if (response is null || response.SourceActionId != actionId)
            {
                return RequireSnapshot(actionId, "The action response is missing or belongs to another action.");
            }

            if (!response.IsAccepted)
            {
                return new GameActionSendResult(actionId, false, null, response.ErrorMessage,
                    validationCode: response.ValidationCode);
            }

            var batchResult = _batchApplier.Apply(response.Batch);
            return new GameActionSendResult(actionId, true, batchResult, batchResult.ErrorMessage);
        }

        private GameActionSendResult CreateCancelledResult(Guid actionId, bool wasSent)
        {
            var batchResult = wasSent && !_disposed ? _batchApplier.Apply(null) : null;
            return new GameActionSendResult(actionId, null, batchResult, null, isCancelled: true);
        }

        private GameActionSendResult RequireSnapshot(Guid actionId, string message)
        {
            return new GameActionSendResult(actionId, null, _batchApplier.Apply(null), message);
        }

        private void NotifyCompleted(GameActionSendResult result)
        {
            if (Completed is not { } handlers)
            {
                return;
            }

            foreach (var action in handlers.GetInvocationList())
            {
                var handler = (Action<GameActionSendResult>)action;
                try
                {
                    handler(result);
                }
                catch (Exception exception)
                {
                    Logger.Error($"Action completion observer failed: {exception}");
                }
            }
        }
    }
}
