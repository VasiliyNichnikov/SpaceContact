using System.Threading;
using System.Threading.Tasks;
using Logs;

namespace Network.Requests
{
    public class NetworkService : INetworkService
    {
        private NetworkServiceObj? _serviceObj;
        
        public void Bind(NetworkServiceObj serviceObj)
        {
            _serviceObj = serviceObj;
        }

        public bool IsLoaded => 
            _serviceObj != null;

        Task<TResponse?> INetworkService.GetDataAsync<TRequest, TResponse>(TRequest requestData, NetworkRequestType requestType, CancellationToken token) where TResponse : class
        {
            if (_serviceObj == null)
            {
                Logger.Error("NetworkService.GetDataAsync: Network service is not initialized.");
                
                return Task.FromResult<TResponse?>(null);
            }
            
            return _serviceObj.GetDataAsync<TRequest, TResponse>(requestData, requestType, token);
        }

        async Task<bool> INetworkService.UpdateDataAsync<TRequest>(TRequest requestData, NetworkRequestType requestType, CancellationToken token) where TRequest : class
        {
            if (_serviceObj == null)
            {
                Logger.Error("NetworkService.UpdateDataAsync: Network service is not initialized.");
                
                return false;
            }
            
            var result = await _serviceObj.GetDataAsync<TRequest, EmptyResponseData>(requestData, requestType, token);
            
            return !token.IsCancellationRequested && result != null;
        }
    }
}