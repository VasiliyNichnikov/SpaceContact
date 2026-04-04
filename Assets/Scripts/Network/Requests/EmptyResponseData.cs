using System;

namespace Network.Requests
{
    [Serializable]
    public class EmptyResponseData
    {
        private static EmptyResponseData? _instance;
        
        public static EmptyResponseData Instance => _instance ??= new EmptyResponseData();
    }
}