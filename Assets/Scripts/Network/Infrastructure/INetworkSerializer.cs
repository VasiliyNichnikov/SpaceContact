using System;

namespace Network.Infrastructure
{
    public interface INetworkSerializer
    {
        byte[] Serialize<T>(T data);
        
        T Deserialize<T>(byte[] data);
        
        object Deserialize(Type type, byte[] data);
    }
}