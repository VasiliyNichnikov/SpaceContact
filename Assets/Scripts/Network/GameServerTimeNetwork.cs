using System;
using Core;
using Unity.Netcode;
using VContainer.Unity;

namespace Network
{
    public class GameServerTimeNetwork : ITickable, IGameServerTime
    {
        public double ServerTimeInSeconds => 
            NetworkManager.Singleton.ServerTime.Time;

        public event Action? Tick;

        void ITickable.Tick() => 
            Tick?.Invoke();
    }
}