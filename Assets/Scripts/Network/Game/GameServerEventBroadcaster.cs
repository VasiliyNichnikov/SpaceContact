using System;
using System.Collections.Generic;
using System.Linq;
using Core.Game;
using Core.Game.Dto;
using Core.Game.Server;
using Logs;
using VContainer.Unity;

namespace Network.Game
{
    public sealed class GameServerEventBroadcaster : IServerEventBroadcaster, ITickable
    {
        private const int MaximumRetryCount = 3;
        private const double RetryDelayInSeconds = 1;
        private const double AcknowledgementTimeoutInSeconds = 10;

        private readonly IGameServerTime _time;
        private readonly ServerClientRevisionTracker _tracker;
        private readonly Dictionary<(ulong PlayerId, long Revision), GameEventDeliveryRetry> _pendingByDelivery = new();
        private GameEventRpcRelayNetwork? _relay;

        public GameServerEventBroadcaster(IGameServerTime time, ServerClientRevisionTracker tracker)
        {
            _time = time;
            _tracker = tracker;
        }

        public void SetRelayObject(GameEventRpcRelayNetwork relay)
        {
            _relay = relay;
        }

        public void SendEvent(ulong recipientPlayerId, GameEventBatchData batchData)
        {
            try
            {
                SendBatch(recipientPlayerId, batchData);
            }
            catch (Exception exception)
            {
                if (_tracker.HasAcknowledgedRevision(recipientPlayerId, batchData.Revision))
                {
                    return;
                }

                var delivery = (PlayerId: recipientPlayerId, Revision: batchData.Revision);
                if (_pendingByDelivery.TryGetValue(delivery, out var pending))
                {
                    // Repeated sends of the same revision must not restart its retry budget.
                    pending.LastError = exception.Message;
                }
                else
                {
                    var eventDeliveryRetry = new GameEventDeliveryRetry(batchData, _time.ServerTimeInSeconds + RetryDelayInSeconds, exception.Message);
                    _pendingByDelivery.Add(delivery, eventDeliveryRetry);
                }

                Logger.Warning($"Delivery of revision {batchData.Revision} to player {recipientPlayerId} failed; retry scheduled: {exception}");
            }
        }

        void ITickable.Tick()
        {
            if (_pendingByDelivery.Count == 0)
            {
                return;
            }

            var now = _time.ServerTimeInSeconds;
            // Sending to the host can synchronously produce ACKs or another outgoing batch.
            foreach (var (delivery, pending) in _pendingByDelivery.ToArray())
            {
                if (_tracker.HasAcknowledgedRevision(delivery.PlayerId, delivery.Revision))
                {
                    _pendingByDelivery.Remove(delivery);
                    continue;
                }

                if (now < pending.NextAttemptTimeInSeconds)
                {
                    continue;
                }

                if (pending.RetryCount >= MaximumRetryCount)
                {
                    FailDelivery(delivery, pending);
                    continue;
                }

                pending.RetryCount++;
                try
                {
                    // Repeat delivery of committed data; never execute the originating action again.
                    SendBatch(delivery.PlayerId, pending.Batch);
                    pending.LastError = "No acknowledgement arrived after sending the batch.";
                    pending.NextAttemptTimeInSeconds = now + AcknowledgementTimeoutInSeconds;
                }
                catch (Exception exception)
                {
                    pending.LastError = exception.Message;
                    pending.NextAttemptTimeInSeconds = now + RetryDelayInSeconds;
                    if (pending.RetryCount >= MaximumRetryCount)
                    {
                        FailDelivery(delivery, pending);
                    }
                }
            }
        }

        private void SendBatch(ulong playerId, GameEventBatchData batch)
        {
            var relay = _relay;
            if (relay == null || !relay.IsSpawned || !relay.IsServer)
            {
                throw new InvalidOperationException("The server event relay is not ready.");
            }

            relay.SendEventsToClients(batch, new[] { playerId });
        }

        private void FailDelivery((ulong PlayerId, long Revision) delivery, GameEventDeliveryRetry pending)
        {
            _pendingByDelivery.Remove(delivery);
            // A send may throw after the host has already applied and acknowledged the batch.
            if (_tracker.HasAcknowledgedRevision(delivery.PlayerId, delivery.Revision))
            {
                return;
            }

            var error = $"Player {delivery.PlayerId} did not acknowledge revision {delivery.Revision} " +
                        $"after {MaximumRetryCount} delivery retries. {pending.LastError}";
            Logger.Error(error);
        }
    }
}
