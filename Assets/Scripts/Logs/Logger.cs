using System;

namespace Logs
{
    public static class Logger
    {
        private static readonly ILogger _impl;

        static Logger()
        {
            _impl = new UnityLogger();
        }
        
        public static void Warning(string message) => _impl.Warning(message);

        public static void Error(string message) => _impl.Error(message);

        public static void Log(string message) => _impl.Log(message);
        
        public static void CriticalError(string message) => _impl.Error(message);

#if UNITY_EDITOR
        [Obsolete("Method only for debug")]
        public static void Todo(string message)
        {
            var readyMessage = $"TODO: {message}";
            Logger.Warning(readyMessage);
        }
#endif
    }
}