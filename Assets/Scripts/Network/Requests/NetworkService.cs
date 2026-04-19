using System.Threading;
using System.Threading.Tasks;
using Logs;

namespace Network.Requests
{
    public sealed class NetworkService : INetworkService
    {
        private readonly TaskCompletionSource<bool> _readyTcs = new();
        
        private NetworkServiceObj? _serviceObj;
        
        public void SetServiceObject(NetworkServiceObj serviceObj)
        {
            _serviceObj = serviceObj;
            _readyTcs.TrySetResult(true);
        }

        Task<TResponse?> INetworkService.GetDataAsync<TRequest, TResponse>(TRequest requestData, NetworkRequestType requestType, CancellationToken token) where TResponse : class
        {
            return GetDataInternalAsync<TRequest, TResponse>(requestData, requestType, token);
        }

        async Task<bool> INetworkService.UpdateDataAsync<TRequest>(TRequest requestData, NetworkRequestType requestType, CancellationToken token) where TRequest : class
        {
            var result = await GetDataInternalAsync<TRequest, EmptyResponseData>(requestData, requestType, token);
            
            return !token.IsCancellationRequested && result != null;
        }

        private async Task<TResponse?> GetDataInternalAsync<TRequest, TResponse>(
            TRequest requestData, 
            NetworkRequestType requestType,
            CancellationToken token) where TResponse : class
        {
            if (_serviceObj == null)
            {
                await _readyTcs.Task;
            }

            if (_serviceObj == null)
            {
                Logger.Error($"{nameof(NetworkService)}.{nameof(GetDataInternalAsync)}: serviceObj is not initialized.");
                
                return null;
            }
            
            return await _serviceObj.GetDataAsync<TRequest, TResponse>(requestData, requestType, token);
        } 
    }
}