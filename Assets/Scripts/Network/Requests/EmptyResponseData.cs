using System;

namespace Network.Requests
{
    [Serializable]
    public class EmptyResponseData
    {
        private static EmptyResponseData? _instance;

        private EmptyResponseData()
        {
            // nothing
        }
        
        public static EmptyResponseData Instance => _instance ??= new EmptyResponseData();
    }
}