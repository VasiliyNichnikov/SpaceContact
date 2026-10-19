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
        private bool _started;
        private bool _disposed;
        private bool _recovering;
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

        public void Start()
        {
            if (_started || _disposed)
            {
                return;
            }

            if (_context.Role != GameContextRole.Client)
            {
                throw new InvalidOperationException("Snapshot recovery requires the client context, including on the host.");
            }

            _started = true;
            _applier.Completed += OnBatchCompleted;
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            _applier.Completed -= OnBatchCompleted;
            _lifetime.Cancel();
            _lifetime.Dispose();
        }

        private void OnBatchCompleted(GameEventBatchApplyResult result, long observedRevision)
        {
            _highestObservedRevision = Math.Max(_highestObservedRevision, observedRevision);
            if (result.Status != GameEventBatchApplyStatus.SnapshotRequired)
            {
                _ack.Send(_context.Revision, _context.Phase.Revision);
                return;
            }

            if (!_recovering)
            {
                _recovering = true;
                RecoverAsync(_lifetime.Token).FireAndForget();
            }
        }

        private async Task RecoverAsync(CancellationToken token)
        {
            var cancelled = new TaskCompletionSource<bool>();
            await using var registration = token.Register(() => cancelled.TrySetResult(true));
            try
            {
                while (!token.IsCancellationRequested)
                {
                    try
                    {
                        var dto = new GameSnapshotRequestDto();
                        var response = await _network.GetDataAsync<GameSnapshotRequestDto, GameSnapshotResponseDto>(dto, NetworkRequestType.GetGameSnapshot, token);
                        
                        if (token.IsCancellationRequested)
                        {
                            return;
                        }

                        // Live batches received during the request are covered by a newer snapshot.
                        // Never ACK a snapshot that leaves an observed revision unapplied.
                        if (response is not null && response.Revision >= _highestObservedRevision &&
                            response.Revision >= _context.Revision)
                        {
                            _context.LoadSnapshot(response.Revision, response.Snapshot);
                            if (token.IsCancellationRequested)
                            {
                                return;
                            }

                            if (_context.IsSynchronizationRequired || _context.Revision < _highestObservedRevision)
                            {
                                continue;
                            }

                            _ack.Send(_context.Revision, _context.Phase.Revision);
                            return;
                        }
                    }
                    catch (Exception exception)
                    {
                        if (!token.IsCancellationRequested)
                        {
                            Logger.Error($"Snapshot recovery failed: {exception}");
                        }
                    }

                    await Task.WhenAny(Task.Delay(RetryDelayMilliseconds, token), cancelled.Task);
                }
            }
            finally
            {
                _recovering = false;
            }
        }
    }
}
