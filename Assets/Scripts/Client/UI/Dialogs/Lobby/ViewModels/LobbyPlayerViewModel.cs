using System;
using Core.Lobby;
using Core.Player;
using CoreConvertor;
using Reactivity;
using UnityEngine;
using Logger = Logs.Logger;

namespace Client.UI.Dialogs.Lobby.ViewModels
{
    public class LobbyPlayerViewModel : IDisposable
    {
        private readonly ReactivityProperty<string> _name = new();
        private readonly ReactivityProperty<Color> _color = new();
        
        private readonly IPlayerManager _player;
        
        public LobbyPlayerViewModel(IPlayerManager player, ILobbyColorProvider colorProvider)
        {
            _player = player;
            ColorSelectionPanelViewModel = new LobbyColorSelectionPanelViewModel(player.SetColor, colorProvider);
            _player.OnPlayerInfoUpdated += PlayerInfoUpdated;
            PlayerInfoUpdated();
        }
        
        public IReactivityProperty<string> Name => 
            _name;

        public IReactivityProperty<Color> Color => 
            _color;

        public bool IsMe => 
            _player.IsCurrentPlayer;
        
        public bool IsOwnerLobby => 
            _player.IsOwnerLobby;
        
        public LobbyColorSelectionPanelViewModel ColorSelectionPanelViewModel { get; }

        public void OnColorSelectionButtonClickHandler()
        {
            if (IsMe)
            {
                ColorSelectionPanelViewModel.OpenOrClosePanel();
            }
        }

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
            ColorSelectionPanelViewModel.Dispose();
            _player.OnPlayerInfoUpdated -= PlayerInfoUpdated;
        }

        private void PlayerInfoUpdated()
        {
            _name.Value = _player.Name;
            _color.Value = ColorConvertor.FromCoreColor(_player.Color);
        }
    }
}