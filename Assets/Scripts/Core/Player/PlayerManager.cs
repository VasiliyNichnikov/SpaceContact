using System;
using Core.EngineData;

namespace Core.Player
{
    public class PlayerManager : IPlayerManager, IPlayerManagerNetwork
    {
        private const string DefaultName = "None";
        private const string DefaultColor = "#FFFFFF";

        public string Name { get; private set; } = DefaultName;

        public Color Color { get; private set; } = Color.FromHex(DefaultColor);
        
        public bool IsCurrentPlayer { get; private set; }

        public event Action? OnPlayerInfoUpdated;

        void IPlayerManagerNetwork.SetLocalStatus(bool isOwner)
        {
            IsCurrentPlayer = isOwner;
        }

        void IPlayerManagerNetwork.SyncNameFromNetwork(string oldValue, string newValue)
        {
            if (string.Equals(oldValue, newValue, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }
            
            Name = newValue;
            OnPlayerInfoUpdated?.Invoke();
        }

        void IPlayerManagerNetwork.SyncColorFromNetwork(Color oldValue, Color newValue)
        {
            if (Equals(oldValue, newValue))
            {
                return;
            }
            
            Color = newValue;
            OnPlayerInfoUpdated?.Invoke();
        }
    }
}