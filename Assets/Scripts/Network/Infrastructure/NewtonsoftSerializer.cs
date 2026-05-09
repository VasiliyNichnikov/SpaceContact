using System;
using Newtonsoft.Json;

namespace Network.Infrastructure
{
    public class NewtonsoftSerializer : INetworkSerializer
    {
        public byte[] Serialize<T>(T data)
        {
            var settings = new JsonSerializerSettings {
                TypeNameHandling = TypeNameHandling.Auto
            };
            var json = JsonConvert.SerializeObject(data, settings);
            
            return System.Text.Encoding.UTF8.GetBytes(json);
        }

        public T Deserialize<T>(byte[] data)
        {
            var json = System.Text.Encoding.UTF8.GetString(data);
            
            return JsonConvert.DeserializeObject<T>(json) ?? throw new InvalidOperationException();
        }

        public object Deserialize(Type type, byte[] data)
        {
            var json = System.Text.Encoding.UTF8.GetString(data);
            
            return JsonConvert.DeserializeObject(json, type) ?? throw new InvalidOperationException();
        }
    }
}