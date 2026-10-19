using System;
using System.Collections.Generic;
using Core.Game;
using Unity.Netcode;
using VContainer.Unity;

namespace Network.Game
{
    public class GameServerTimeNetwork : ITickable, IGameServerTime, IDisposable
    {
        private readonly List<IGameServerTime.Update> _actions = new();
        private readonly NetworkManager _networkManager;
        private double _previousTime;

        public GameServerTimeNetwork(NetworkManager networkManager)
        {
            _networkManager = networkManager;
        }

        public double ServerTimeInSeconds => 
            _networkManager.ServerTime.Time;

        public void Subscribe(IGameServerTime.Update action)
        {
            if(_actions.Contains(action))
            {
                return;
            }
            
            _actions.Add(action);
        }

        public void Unsubscribe(IGameServerTime.Update action)
        {
            if (!_actions.Contains(action))
            {
                return;
            }
            
            _actions.Remove(action);
        }

        void ITickable.Tick()
        {
            var currentServerTime = ServerTimeInSeconds;
            var deltaTime = currentServerTime - _previousTime;

            foreach (var action in _actions)
            {
                action.Invoke(deltaTime);
            }
            
            _previousTime = currentServerTime;
        }

        void IDisposable.Dispose()
        {
            _actions.Clear();
        }
    }
}
