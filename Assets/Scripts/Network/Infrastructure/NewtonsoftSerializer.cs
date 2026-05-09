using System;
using Newtonsoft.Json;

namespace Network.Infrastructure
{
    public class NewtonsoftSerializer : INetworkSerializer
    {
        private static readonly JsonSerializerSettings _settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto
        };

        public byte[] Serialize<T>(T data)
        {
            var json = JsonConvert.SerializeObject(data, _settings);
            
            return System.Text.Encoding.UTF8.GetBytes(json);
        }

        public T Deserialize<T>(byte[] data)
        {
            var json = System.Text.Encoding.UTF8.GetString(data);

            return JsonConvert.DeserializeObject<T>(json, _settings) ?? throw new InvalidOperationException();
        }

        public object Deserialize(Type type, byte[] data)
        {
            var json = System.Text.Encoding.UTF8.GetString(data);

            return JsonConvert.DeserializeObject(json, type, _settings) ?? throw new InvalidOperationException();
        }
    }
}