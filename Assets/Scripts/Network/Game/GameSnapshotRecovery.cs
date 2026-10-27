using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Game;
using Core.Game.Dto;
using Core.Game.Events;
using GeneralUtils;
using Logs;
using Network.Requests;

namespace Network.Game
{
    public sealed class GameSnapshotRecovery : IDisposable
    {
        private const int RetryDelayMilliseconds = 1000;

        private readonly GameContext _context;
        private readonly GameEventBatchApplier _applier;
        private readonly INetworkService _network;
        private readonly GameRevisionAckNetwork _ack;
        private readonly CancellationTokenSource _lifetime = new();
        private GameSnapshotRecoveryState _state;
        private long _highestObservedRevision;

        public GameSnapshotRecovery(
            GameContext context, 
            GameEventBatchApplier applier,
            INetworkService network, 
            GameRevisionAckNetwork ack)
        {
            _context = context;
            _applier = applier;
            _network = network;
            _ack = ack;
        }

        private bool RequiresSnapshot =>
            _context.IsSynchronizationRequired || _context.Revision < _highestObservedRevision;

        public void Start()
        {
            if (_state.IsStarted || _state.IsDisposed)
            {
                return;
            }

            if (_context.Role != GameContextRole.Client)
            {
                throw new InvalidOperationException("Snapshot recovery requires the client context, including on the host.");
            }

            _state.IsStarted = true;
            _applier.Completed += OnBatchCompleted;
        }

        public void AllowSynchronization()
        {
            if (_state.IsDisposed)
            {
                return;
            }

            Start();
            _state.IsSynchronizationAllowed = true;
            TryStartRecovery();
        }

        public void Dispose()
        {
            _state.IsDisposed = true;
            _applier.Completed -= OnBatchCompleted;
            _lifetime.Cancel();
            _lifetime.Dispose();
        }

        private static async Task ObserveAbandonedRequestAsync(Task request)
        {
            try
            {
                await request;
            }
            catch (Exception exception)
            {
                Logger.Error($"Snapshot request failed after synchronization was disposed: {exception}");
            }
        }

        private void OnBatchCompleted(GameEventBatchApplyResult result, long observedRevision)
        {
            if (_state.IsDisposed)
            {
                return;
            }

            _highestObservedRevision = Math.Max(_highestObservedRevision, observedRevision);
            if (!_state.IsSynchronizationAllowed)
            {
                return;
            }

            if (result.Status == GameEventBatchApplyStatus.SnapshotRequired || RequiresSnapshot)
            {
                TryStartRecovery();
                return;
            }

            _ack.Send(_context.Revision, _context.Phase.Revision);
        }

        private void TryStartRecovery()
        {
            if (_state.IsRecovering || !RequiresSnapshot)
            {
                return;
            }

            _state.IsRecovering = true;
            RecoverAsync(_lifetime.Token).FireAndForget();
        }

        private async Task RecoverAsync(CancellationToken token)
        {
            var cancelled = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            await using var registration = token.Register(() => cancelled.TrySetResult(true));
            try
            {
                while (!token.IsCancellationRequested)
                {
                    try
                    {
                        var dto = new GameSnapshotRequestDto();
                        var request = _network.GetDataAsync<GameSnapshotRequestDto, GameSnapshotResponseDto>(
                            dto, NetworkRequestType.GetGameSnapshot, token);
                        await Task.WhenAny(request, cancelled.Task);
                        
                        if (token.IsCancellationRequested)
                        {
                            // Do not wait for a transport that ignores cancellation, or apply its late result.
                            ObserveAbandonedRequestAsync(request).FireAndForget();
                            return;
                        }

                        var response = await request;
                        
                        if (token.IsCancellationRequested)
                        {
                            return;
                        }

                        // Live batches received during the request must be covered before loading and ACKing.
                        if (response != null && response.Revision >= _highestObservedRevision &&
                            response.Revision >= _context.Revision)
                        {
                            _context.LoadSnapshot(response.Revision, response.Snapshot);
                            if (token.IsCancellationRequested)
                            {
                                return;
                            }

                            if (!RequiresSnapshot)
                            {
                                _ack.Send(_context.Revision, _context.Phase.Revision);
                                // Host ACK handling may synchronously deliver another batch.
                                if (token.IsCancellationRequested || !RequiresSnapshot)
                                {
                                    return;
                                }
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        if (!token.IsCancellationRequested)
                        {
                            Logger.Error($"Snapshot synchronization failed: {exception}");
                        }
                    }

                    if (token.IsCancellationRequested)
                    {
                        return;
                    }

                    // Cancellation completes normally; Task.Delay never receives a cancelled token.
                    await Task.WhenAny(Task.Delay(RetryDelayMilliseconds, token), cancelled.Task);
                }
            }
            finally
            {
                _state.IsRecovering = false;
            }
        }
    }
}
