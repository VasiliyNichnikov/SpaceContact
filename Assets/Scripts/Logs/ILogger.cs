namespace Logs
{
    internal interface ILogger
    {
        void Warning(string message);

        void Error(string message);

        void Log(string message);
    }
}