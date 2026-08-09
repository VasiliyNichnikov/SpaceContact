using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Game.Events;
using GeneralUtils;
using Logs;

namespace Core.Game.Actions
{
    public sealed class GameActionClient : IDisposable
    {
        private readonly IGameActionServerInteraction _network;
        private readonly GameContext _context;
        private readonly PendingGameActions _pendingActions = new();
        private readonly CancellationTokenSource _cts = new();

        public GameActionClient(
            IGameActionServerInteraction network,
            GameContext context)
        {
            _network = network;
            _context = context;
        }

        public void Send(IGameAction action)
        {
            SendAsync(action, _cts.Token).FireAndForget();
        }

        public async Task SendAsync(IGameAction action, CancellationToken ct)
        {
            _pendingActions.Add(action);

            var actor = GameActor.Player(_context.LocalPlayerId);
            var response = await _network.SendAsync(action, actor, ct);

            if (response == null || ct.IsCancellationRequested)
            {
                _pendingActions.Cancel(action.Id);
                return;
            }
            
            _pendingActions.Complete(action.Id);

            if (!response.IsAccepted)
            {
                Logger.Error($"{nameof(GameActionClient)}.{nameof(SendAsync)}: couldn't get server data. " +
                             $"ErrorMessage: {response.ErrorMessage}");
                return;
            }

            var events = response.Batch?.Events ?? Array.Empty<IGameEvent>();
            
            foreach (var gameEvent in events)
            {
                gameEvent.Apply(_context);
            }
        }

        public void Dispose()
        {
            _cts.Cancel();
            _cts.Dispose();
        }
    }
}