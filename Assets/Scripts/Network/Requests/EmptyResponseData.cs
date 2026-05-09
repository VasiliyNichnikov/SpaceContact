using System;
using Newtonsoft.Json;

namespace Network.Requests
{
    [Serializable]
    public class EmptyResponseData
    {
        private static EmptyResponseData? _instance;

        [JsonConstructor]
        private EmptyResponseData()
        {
            // nothing
        }
        
        public static EmptyResponseData Instance => _instance ??= new EmptyResponseData();
    }
}