using System;
using Core.Player;
using CoreConvertor;
using Reactivity;
using UnityEngine;
using Logger = Logs.Logger;

namespace Client.UI.Dialogs.Lobby
{
    public class LobbyPlayerViewModel : IDisposable
    {
        private readonly ReactivityProperty<string> _name = new();
        
        private readonly IPlayerManager _player;
        
        public LobbyPlayerViewModel(IPlayerManager player)
        {
            _player = player;
            _name.Value = player.Name;
            _player.OnPlayerInfoUpdated += PlayerInfoUpdated;
        }
        
        public IReactivityProperty<string> Name => _name;

        public Color Color => ColorConvertor.FromCoreColor(_player.Color);

        public bool IsMe => _player.IsCurrentPlayer;
        
        public bool IsOwnerLobby => _player.IsOwnerLobby;

        public void ChangeNameClickHandler(string newName)
        {
            if (string.Equals(newName, _player.Name, StringComparison.Ordinal))
            {
                return;
            }
            
            _player.SetName(newName);
        }

        public void ToLeaveButtonClick()
        {
            Logger.Warning("To leave button click.Need support.");
        }

        public void Dispose()
        {
            _player.OnPlayerInfoUpdated -= PlayerInfoUpdated;
        }

        private void PlayerInfoUpdated()
        {
            _name.Value = _player.Name;
        }
    }
}