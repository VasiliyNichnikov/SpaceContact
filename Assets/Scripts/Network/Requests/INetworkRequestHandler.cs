namespace Network.Requests
{
    public interface INetworkRequestHandler
    {
        NetworkRequestType Type { get; }
        
        byte[] Handle(byte[] requestBytes);
    }
}