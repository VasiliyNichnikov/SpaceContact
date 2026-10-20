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
        private bool _isDisposed;

        public GameActionClient(IGameActionServerInteraction network, GameContext context,
            PendingGameActions pendingActions, GameEventBatchApplier batchApplier)
        {
            _network = network;
            _context = context;
            _pendingActions = pendingActions;
            _batchApplier = batchApplier;
        }

        public event Action<GameActionSendResult>? Completed;

        public void Send(ClientGameAction action)
        {
            SendAsync(action, CancellationToken.None).FireAndForget(exception => Logger.Error($"Action send failed: {exception}"));
        }

        private async Task<GameActionSendResult> SendAsync(ClientGameAction action, CancellationToken ct)
        {
            EnsureCanSend();
            var actionId = action.Id;

            if (ct.IsCancellationRequested)
            {
                return CreateCancelledResult(actionId, false);
            }

            if (actionId == Guid.Empty)
            {
                throw new ArgumentException("An action must have a non-empty ID.", nameof(action));
            }

            if (!_pendingActions.TryAdd(action))
            {
                throw new InvalidOperationException($"Action {actionId} is already pending.");
            }

            GameActionSendResult? result = null;
            var wasSent = false;
            using var cancellation = CancellationTokenSource.CreateLinkedTokenSource(ct, _lifetimeCancellation.Token);

            try
            {
                EnsureCanSend();
                wasSent = true;
                var response = await _network.SendAsync(action, GameActor.Player(_context.LocalPlayerId), cancellation.Token);

                if (cancellation.IsCancellationRequested)
                {
                    result = CreateCancelledResult(actionId, wasSent);
                    return result;
                }

                result = ProcessResponse(actionId, response);
                return result;
            }
            catch (Exception exception)
            {
                if (wasSent && !_isDisposed)
                {
                    result = RequireSnapshot(actionId, exception.Message);
                }

                throw;
            }
            finally
            {
                _pendingActions.Remove(actionId);
                if (result is not null && !_isDisposed)
                {
                    NotifyCompleted(result);
                }
            }
        }

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _isDisposed = true;
            _lifetimeCancellation.Cancel();
            _lifetimeCancellation.Dispose();
        }

        private void EnsureCanSend()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(nameof(GameActionClient));
            }

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
            var batchResult = wasSent && !_isDisposed ? _batchApplier.Apply(null) : null;
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
