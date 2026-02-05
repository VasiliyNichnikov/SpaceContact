using System;

namespace Network.Infrastructure
{
    public class JsonNetworkSerializer : INetworkSerializer
    {
        public byte[] Serialize<T>(T data)
        {
            throw new System.NotImplementedException();
        }

        public T Deserialize<T>(byte[] data)
        {
            throw new System.NotImplementedException();
        }

        public object Deserialize(Type type, byte[] data)
        {
            throw new NotImplementedException();
        }
    }
}