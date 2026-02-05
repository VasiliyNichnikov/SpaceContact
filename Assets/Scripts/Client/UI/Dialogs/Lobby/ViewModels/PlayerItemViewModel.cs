using System;
using Core.Player;
using CoreConvertor;
using UnityEngine;

namespace Client.UI.Dialogs.Lobby
{
    public class PlayerItemViewModel : IDisposable
    {
        private readonly IPlayerManager _corePlayer;
        
        public PlayerItemViewModel(IPlayerManager corePlayer)
        {
            _corePlayer = corePlayer;
            PlayerInfoUpdated();

            _corePlayer.OnPlayerInfoUpdated += PlayerInfoUpdated;
        }

        public string Name { get; private set; } = null!;
        
        public Color Color { get; private set; }

        public void Dispose()
        {
            _corePlayer.OnPlayerInfoUpdated -= PlayerInfoUpdated;
        }

        private void PlayerInfoUpdated()
        {
            Name = _corePlayer.Name;
            Color = ColorConvertor.FromCoreColor(_corePlayer.Color);
        }
    }
}