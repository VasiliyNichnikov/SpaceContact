using System;

namespace Core
{
    public interface IGameServerTime
    {
        double ServerTimeInSeconds { get; }

        event Action? Tick;
    }
}