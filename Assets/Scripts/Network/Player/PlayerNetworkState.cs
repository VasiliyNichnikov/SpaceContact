using Core.Player;
using CoreConvertor;
using Unity.Netcode;
using Unity.Collections;
using UnityEngine;
using VContainer;

namespace Network.Player
{
    /// <summary>
    /// Паспорт игрока.
    /// Хранит данные игрока в текущей сессии
    /// </summary>
    public class PlayerNetworkState : NetworkBehaviour
    {
        private readonly NetworkVariable<FixedString64Bytes> _playerName = new();
        private readonly NetworkVariable<Color> _playerColor = new();
        
        private IPlayerManager _corePlayer = null!;
        private IPlayerManagerNetwork _corePlayerNetwork = null!;
        private PlayersRegistry _registry = null!;
        
        [Inject]
        private void Construct(
            IPlayerManager playerManager, 
            IPlayerManagerNetwork playerManagerNetwork,
            PlayersRegistry registry)
        {
            _corePlayer = playerManager;
            _corePlayerNetwork = playerManagerNetwork;
            _registry = registry;
        }
        
        public IPlayerManager CorePlayer => _corePlayer;

        public override void OnNetworkSpawn()
        {
            _corePlayerNetwork.SetLocalStatus(IsOwner);
            
            if (IsServer)
            {
                _corePlayer.OnPlayerInfoUpdated += UpdatePlayerInfo;
            }
            
            _playerName.OnValueChanged += OnNetworkPlayerNameValueChanged;
            _playerColor.OnValueChanged += OnNetworkPlayerColorValueChanged;
            
            _registry.Register(this);
        }

        public override void OnNetworkDespawn()
        {
            if (IsServer)
            {
                _corePlayer.OnPlayerInfoUpdated -= UpdatePlayerInfo;
            }
            
            _playerName.OnValueChanged -= OnNetworkPlayerNameValueChanged;
            _playerColor.OnValueChanged -= OnNetworkPlayerColorValueChanged;
            _registry.Unregister(this);
        }

        private void UpdatePlayerInfo()
        {
            _playerName.Value = _corePlayer.Name;
            _playerColor.Value = ColorConvertor.FromCoreColor(_corePlayer.Color);
        }

        private void OnNetworkPlayerNameValueChanged(FixedString64Bytes oldValue, FixedString64Bytes newName) =>
            _corePlayerNetwork.SyncNameFromNetwork(oldValue.Value, newName.Value);

        private void OnNetworkPlayerColorValueChanged(Color oldValue, Color newValue)
        {
            var oldColor = ColorConvertor.FromUnityColor(oldValue);
            var newColor = ColorConvertor.FromUnityColor(newValue);
            
            _corePlayerNetwork.SyncColorFromNetwork(oldColor, newColor);
        }
    }
}