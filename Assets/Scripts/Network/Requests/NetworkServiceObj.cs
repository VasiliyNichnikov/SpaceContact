using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Logs;
using Network.Infrastructure;
using Unity.Netcode;
using VContainer;

namespace Network.Requests
{
    public class NetworkServiceObj : NetworkBehaviour
    {
        private const int RequestTimeoutMilliseconds = 10000;

        private readonly Dictionary<ulong, TaskCompletionSource<byte[]?>> _pendingRequests = new();
        private INetworkSerializer _serializer = null!;
        private NetworkRequestRouter _router = null!;
        private ulong _nextRequestId;

        public override void OnNetworkDespawn()
        {
            foreach (var completion in new List<TaskCompletionSource<byte[]?>>(_pendingRequests.Values))
            {
                completion.TrySetResult(null);
            }

            _pendingRequests.Clear();
            base.OnNetworkDespawn();
        }

        public async Task<TResponse?> GetDataAsync<TRequest, TResponse>(TRequest requestData,
            NetworkRequestType requestType, CancellationToken token) where TResponse : class
        {
            if (token.IsCancellationRequested || !IsSpawned || !IsClient)
            {
                return null;
            }

            var requestId = _nextRequestId++;
            var completion = new TaskCompletionSource<byte[]?>();
            _pendingRequests.Add(requestId, completion);
            try
            {
                await using var registration = token.Register(() => completion.TrySetResult(null));
                var requestBytes = _serializer.Serialize(requestData);
                if (token.IsCancellationRequested)
                {
                    return null;
                }

                RequestServerRpc(requestId, requestType, requestBytes);
                if (await Task.WhenAny(completion.Task, Task.Delay(RequestTimeoutMilliseconds, token)) != completion.Task)
                {
                    return null;
                }

                var responseBytes = await completion.Task;
                if (token.IsCancellationRequested || responseBytes is null || responseBytes.Length == 0)
                {
                    return null;
                }

                return _serializer.Deserialize<TResponse>(responseBytes);
            }
            finally
            {
                _pendingRequests.Remove(requestId);
            }
        }

        [Inject]
        private void Construct(INetworkSerializer serializer, NetworkRequestRouter router, NetworkService networkService)
        {
            _serializer = serializer;
            _router = router;
            networkService.SetServiceObject(this);
        }

        [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
        private void RequestServerRpc(ulong requestId, NetworkRequestType requestType, byte[] requestBytes,
            RpcParams rpcParams = default)
        {
            var clientId = rpcParams.Receive.SenderClientId;
            byte[] responseBytes;
            try
            {
                responseBytes = _router.Route(requestType, requestBytes, clientId);
            }
            catch (Exception exception)
            {
                Logger.Error($"Network request {requestType} from {clientId} failed: {exception}");
                responseBytes = Array.Empty<byte>();
            }

            var clientParams = new ClientRpcParams
            {
                Send = new ClientRpcSendParams { TargetClientIds = new[] { clientId } }
            };
            ResponseClientRpc(requestId, responseBytes, clientParams);
        }

        [ClientRpc]
        private void ResponseClientRpc(ulong requestId, byte[] responseBytes, ClientRpcParams rpcParams = default)
        {
            // Cancellation, timeout and despawn remove pending entries; late responses are harmless.
            if (_pendingRequests.TryGetValue(requestId, out var completion))
            {
                completion.TrySetResult(responseBytes);
            }
        }
    }
}
