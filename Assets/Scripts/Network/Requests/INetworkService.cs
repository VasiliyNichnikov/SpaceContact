using System.Threading;
using System.Threading.Tasks;

namespace Network.Requests
{
    public interface INetworkService
    {
        Task<TResponse?> GetDataAsync<TRequest, TResponse>(
            TRequest requestData,
            NetworkRequestType requestType,
            CancellationToken token) where TResponse : class;
    }
}