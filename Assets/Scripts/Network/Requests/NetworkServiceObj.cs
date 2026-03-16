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
        private INetworkSerializer _serializer = null!;
        private NetworkRequestRouter _router = null!;

        private readonly Dictionary<ulong, TaskCompletionSource<byte[]>> _pendingRequests = new();
        private ulong _nextRequestId;

        [Inject]
        private void Constructor(
            INetworkSerializer serializer, 
            NetworkRequestRouter router, 
            INetworkService networkService)
        {
            _serializer = serializer;
            _router = router;
            ((NetworkService)networkService).Bind(this);
        }
        
        public async Task<TResponse?> GetDataAsync<TRequest, TResponse>(
            TRequest requestData, 
            NetworkRequestType requestType,
            CancellationToken token) where TResponse : class
        {
            var requestId = _nextRequestId++;
            var tcs = new TaskCompletionSource<byte[]>();
            _pendingRequests.Add(requestId, tcs);
            
            var requestBytes = _serializer.Serialize(requestData);
            RequestServerRpc(requestId, requestType, requestBytes);

            try
            {
                await using (token.Register(() => tcs.TrySetCanceled()))
                {
                    var responseBytes = await tcs.Task;
                    return (TResponse)_serializer.Deserialize(typeof(TResponse), responseBytes);
                }
            }
            catch (OperationCanceledException)
            {
                Logger.Error($"NetworkServiceObj.GetDataAsync: Request {requestId} was canceled.");
                return null;
            }
            finally
            {
                _pendingRequests.Remove(requestId);
            }
        }

        #region Server

        [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
        private void RequestServerRpc(ulong requestId, NetworkRequestType requestType, byte[] requestBytes, RpcParams rpcParams = default)
        {
            var clientId = rpcParams.Receive.SenderClientId;
            var responseBytes = _router.Route(requestType, requestBytes);
            var clientParams = new ClientRpcParams
            {
                Send = new ClientRpcSendParams { TargetClientIds = new[] { clientId } }
            };
            
            ResponseClientRpc(requestId, responseBytes, clientParams);
        }

        #endregion

        [ClientRpc]
        private void ResponseClientRpc(ulong requestId, byte[] responseBytes, ClientRpcParams _ = default)
        {
            if (_pendingRequests.TryGetValue(requestId, out var tcs))
            {
                tcs.TrySetResult(responseBytes);
            }
            else
            {
                Logger.Error($"NetworkServiceObj.ResponseClient: Received response for unknown request ID: {requestId}.");
            }
        }
    }
}