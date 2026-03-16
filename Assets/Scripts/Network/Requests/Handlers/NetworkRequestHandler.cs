using System;
using Logs;
using Network.Infrastructure;

namespace Network.Requests
{
    public abstract class NetworkRequestHandler<TRequest, TResponse> : INetworkRequestHandler
        where TRequest: class
        where TResponse: class
    {
        private readonly INetworkSerializer _serializer;

        protected NetworkRequestHandler(INetworkSerializer serializer)
        {
            _serializer = serializer;
        }
        
        public abstract NetworkRequestType Type { get; }
        
        public byte[] Handle(byte[] requestBytes)
        {
            var request = _serializer.Deserialize<TRequest>(requestBytes);
            var response = ProcessRequest(request);

            if (response == null)
            {
                Logger.Error($"NetworkRequestHandler.Handle: response is null. RequestType: {Type}.");
                
                return Array.Empty<byte>();
            }
            
            return _serializer.Serialize(response);
        }
        
        protected abstract TResponse? ProcessRequest(TRequest request);
    }
}