using System;

namespace Network.Infrastructure
{
    public class JsonNetworkSerializer : INetworkSerializer
    {
        public byte[] Serialize<T>(T data)
        {
            var json = UnityEngine.JsonUtility.ToJson(data);
            
            return System.Text.Encoding.UTF8.GetBytes(json);
        }

        public T Deserialize<T>(byte[] data)
        {
            var json = System.Text.Encoding.UTF8.GetString(data);
            
            return UnityEngine.JsonUtility.FromJson<T>(json);
        }

        public object Deserialize(Type type, byte[] data)
        {
            var json = System.Text.Encoding.UTF8.GetString(data);
            
            return UnityEngine.JsonUtility.FromJson(json, type);
        }
    }
}