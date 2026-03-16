using System;
using System.Collections.Generic;
using Logs;
using Unity.Netcode;

namespace Network
{
    public class PrefabInitializerOnClients : IPrefabInitializerOnClients
    {
        private ulong? _prefabId;
        private readonly HashSet<ulong> _clientsWhoLoadedThis = new();
        
        public event Action<ulong>? OnLoaded;
        
        public bool IsLoaded { get; private set; }

        public void SetPrefabId(ulong prefabId)
        {
            _prefabId = prefabId;
        }

        public void LoadOnClient(ulong clientId)
        {
            _clientsWhoLoadedThis.Add(clientId);
            CheckClientsFullLoaded();
        }

        private void CheckClientsFullLoaded()
        {
            if (IsLoaded)
            {
                Logger.Error("PrefabInitializerOnClients.CheckClientsFullLoaded: ");
            }
            
            var totalConnected = NetworkManager.Singleton.ConnectedClientsIds.Count;

            if (_clientsWhoLoadedThis.Count < totalConnected)
            {
                return;
            }
            
            IsLoaded = true;

            if (_prefabId == null)
            {
                Logger.Error("PrefabInitializerOnClients.CheckClientsFullLoaded: prefabId is null.");
            }
            
            OnLoaded?.Invoke(_prefabId ?? 0);
        }
    }
}