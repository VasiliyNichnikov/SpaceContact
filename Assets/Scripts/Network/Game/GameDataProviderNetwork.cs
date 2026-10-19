using Core.Game;
using Unity.Netcode;

namespace Network.Game
{
    public sealed class GameDataProviderNetwork : IGameDataProvider
    {
        private readonly NetworkManager _networkManager;

        public GameDataProviderNetwork(NetworkManager networkManager)
        {
            _networkManager = networkManager;
        }

        public ulong LocalPlayerId => _networkManager.LocalClientId;
    }
}
