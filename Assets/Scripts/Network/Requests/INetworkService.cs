using System.Threading;
using System.Threading.Tasks;

namespace Network.Requests
{
    public interface INetworkService
    {
        bool IsLoaded { get; }
        
        Task<TResponse?> GetDataAsync<TRequest, TResponse>(
            TRequest requestData,
            NetworkRequestType requestType,
            CancellationToken token) where TResponse : class;
        
        Task<bool> UpdateDataAsync<TRequest>(
            TRequest requestData, 
            NetworkRequestType requestType, 
            CancellationToken token) where TRequest : class;
    }
}