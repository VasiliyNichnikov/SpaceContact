using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Core.Game;
using Core.Game.Actions;
using Core.Game.Server;
using Network.Infrastructure;
using Network.Requests;

namespace App.Game.Services
{
    public sealed class GameServerRequestsRegisterService : IDisposable
    {
        private readonly NetworkRequestRouter _router;
        private readonly INetworkSerializer _networkSerializer;
        private readonly GameStateProjector _projector;
        private readonly GameActionExecutor _executor;
        private readonly IServerEventBroadcaster _broadcaster;

        private ReadOnlyCollection<INetworkRequestHandler>? _handlers;

        public GameServerRequestsRegisterService(
            NetworkRequestRouter router,
            INetworkSerializer networkSerializer,
            GameStateProjector projector,
            GameActionExecutor executor,
            IServerEventBroadcaster broadcaster)
        {
            _router = router;
            _networkSerializer = networkSerializer;
            _projector = projector;
            _executor = executor;
            _broadcaster = broadcaster;
        }

        public void ConfigureRegistry()
        {
            if (_handlers is not null)
            {
                return;
            }

            _handlers = CreateHandlers();
            _router.Subscribe(_handlers);
        }

        public void Dispose()
        {
            if (_handlers is not null)
            {
                _router.Unsubscribe(_handlers);
                _handlers = null;
            }
        }

        private ReadOnlyCollection<INetworkRequestHandler> CreateHandlers()
        {
            var getGameSnapshotHandler = new GetGameSnapshotStateNetworkRequestHandler(_networkSerializer, _projector);
            var applyGameActionStateHandler = new ApplyGameActionNetworkRequestHandler(_networkSerializer, _executor, _broadcaster);

            var allStates = new List<INetworkRequestHandler>
            {
                getGameSnapshotHandler,
                applyGameActionStateHandler,
            };

            return allStates.AsReadOnly();
        }
    }
}
