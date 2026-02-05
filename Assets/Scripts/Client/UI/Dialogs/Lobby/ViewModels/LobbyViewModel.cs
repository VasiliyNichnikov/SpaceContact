using System;
using System.Linq;
using Network.Player;

namespace Client.UI.Dialogs.Lobby
{
    public class LobbyViewModel : IDisposable
    {
        private readonly PlayersRegistry _playersRegistry;
        
        public LobbyViewModel(PlayersRegistry playersRegistry)
        {
            _playersRegistry = playersRegistry;
            _playersRegistry.OnPlayerJoined += PlayerJoined;
            _playersRegistry.OnPlayerLeft += PlayerLeft;
        }
        
        /// <summary>
        /// TODO: пока для теста
        /// </summary>
        public string Name => _playersRegistry.GetAllPlayers().First(p => p.IsOwner).CorePlayer.Name;

        public void Dispose()
        {
            _playersRegistry.OnPlayerJoined -= PlayerJoined;
            _playersRegistry.OnPlayerLeft -= PlayerLeft;
        }

        private void PlayerJoined(PlayerNetworkState _)
        {
            
        }

        private void PlayerLeft(PlayerNetworkState _)
        {
            
        }
    }
}