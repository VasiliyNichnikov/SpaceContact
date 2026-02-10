using Core.Player;
using CoreConvertor;
using Unity.Collections;
using Unity.Netcode;
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

        public override void OnNetworkSpawn()
        {
            var isHost = OwnerClientId == NetworkManager.ServerClientId;
            _corePlayerNetwork.SetLocalStatus(OwnerClientId, IsOwner, isHost);

            if (IsServer || IsOwner)
            {
                _corePlayer.OnPlayerInfoUpdated += OnCoreInfoChanged;
            }

            _playerName.OnValueChanged += OnNetworkPlayerNameValueChanged;
            _playerColor.OnValueChanged += OnNetworkPlayerColorValueChanged;

            _registry.Register(_corePlayer);
        }

        public override void OnNetworkDespawn()
        {
            if (IsServer || IsOwner)
            {
                _corePlayer.OnPlayerInfoUpdated -= OnCoreInfoChanged;
            }

            _playerName.OnValueChanged -= OnNetworkPlayerNameValueChanged;
            _playerColor.OnValueChanged -= OnNetworkPlayerColorValueChanged;
            _registry.Unregister(_corePlayer);
        }

        private void OnCoreInfoChanged()
        {
            var newName = new FixedString64Bytes(_corePlayer.Name);
            var newColor = ColorConvertor.FromCoreColor(_corePlayer.Color);

            if (IsServer)
            {
                _playerName.Value = newName;
                _playerColor.Value = newColor;
            }
            else
            {
                SetPlayerInfoServerRpc(newName, newColor);
            }
        }

        /// <summary>
        /// Данный код выполняется только на сервере
        /// </summary>
        [ServerRpc]
        private void SetPlayerInfoServerRpc(FixedString64Bytes newName, Color color)
        {
            _playerName.Value = newName;
            _playerColor.Value = color;
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