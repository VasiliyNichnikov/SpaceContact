using System;
using System.Collections.Generic;
using System.Linq;
using Logs;

namespace Network.Requests
{
    public class NetworkRequestRouter
    {
        private readonly Dictionary<NetworkRequestType, INetworkRequestHandler> _handlers;

        public NetworkRequestRouter(IEnumerable<INetworkRequestHandler> handlers)
        {
            _handlers = handlers.ToDictionary(h => h.Type, h => h);
        }

        public void Subscribe(IEnumerable<INetworkRequestHandler> handlers)
        {
            foreach (var handler in handlers)
            {
                _handlers.Add(handler.Type, handler);
            }
        }
        
        public void Unsubscribe(IEnumerable<INetworkRequestHandler> handlers)
        {
            foreach (var handler in handlers)
            {
                _handlers.Remove(handler.Type);
            }
        }

        public byte[] Route(NetworkRequestType requestType, byte[] payload)
        {
            if (_handlers.TryGetValue(requestType, out var handler))
            {
                return handler.Handle(payload);
            }
            
            Logger.Error($"NetworkRequestRouter.Route: Not handler register for {requestType}.");
            return Array.Empty<byte>();
        }
    }
}