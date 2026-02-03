#if UNITY_EDITOR || DEBUG
#nullable enable
using UnityEngine;

namespace Logs
{
    internal class UnityLogger : ILogger
    {
        public void Warning(string message)
        {
            Debug.LogWarning(message);
        }

        public void Error(string message)
        {
            Debug.LogError(message);
        }

        public void Log(string message)
        {
            Debug.Log(message);
        }
    }
}
#endif