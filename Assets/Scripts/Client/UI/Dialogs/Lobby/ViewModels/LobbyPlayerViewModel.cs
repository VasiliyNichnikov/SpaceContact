using System;
using System.Threading.Tasks;
using Core.User;
using CoreConvertor;
using GeneralUtils;
using Reactivity;
using UnityEngine;
using Logger = Logs.Logger;

namespace Client.UI.Dialogs.Lobby.ViewModels
{
    public class LobbyPlayerViewModel : ILobbySlotViewModel, IDisposable
    {
        private readonly ReactivityProperty<string> _name = new();
        private readonly ReactivityProperty<Color> _color = new();

        private readonly IUsersColorProvider _colorProvider;
        private readonly IUserServerInteraction _userServerInteraction;
        private readonly IUser _user;

        private bool _isWaitingResponse;
        
        public LobbyPlayerViewModel(
            IUser user, 
            IUsersColorProvider colorProvider,
            IUserServerInteraction userServerInteraction)
        {
            _user = user;
            _colorProvider = colorProvider;
            _userServerInteraction = userServerInteraction;
            ColorSelectionPanelViewModel = new LobbyColorSelectionPanelViewModel(ChangeColor, colorProvider);
            _user.Changed += UserInfoUpdated;
            UserInfoUpdated();
        }
        
        public IReactivityProperty<string> Name => 
            _name;

        public IReactivityProperty<Color> Color => 
            _color;

        public bool IsMe => 
            _user.IsCurrentPlayer;
        
        public bool IsOwnerLobby => 
            _user.IsOwnerLobby;
        
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
            if (_isWaitingResponse)
            {
                return;
            }
            
            if (string.Equals(newName, _user.Name, StringComparison.Ordinal))
            {
                return;
            }
            
            ChangeNameAsync(newName).FireAndForget();
        }

        public void ToLeaveButtonClick()
        {
            Logger.Warning("To leave button click.Need support.");
        }

        public void Dispose()
        {
            ColorSelectionPanelViewModel.Dispose();
            _user.Changed -= UserInfoUpdated;
        }

        private void UserInfoUpdated()
        {
            _name.Value = _user.Name;
            var color = _colorProvider.GetColor(_user.ColorId);
            _color.Value = ColorConvertor.FromCoreColor(color);
        }

        private void ChangeColor(int colorId)
        {
            if (_isWaitingResponse)
            {
                return;
            }
            
            ChangeColorAsync(colorId).FireAndForget();
        }

        private async Task ChangeColorAsync(int colorId)
        {
            if (_isWaitingResponse)
            {
                Logger.Error("LobbyPlayerViewModel.ChangeColorAsync: wait for a response from the server.");
                return;
            }
            
            _isWaitingResponse = true;
            await _userServerInteraction.ChangeMyColorAsync(colorId);
            _isWaitingResponse = false;
        }
        
        private async Task ChangeNameAsync(string newName)
        {
            if (_isWaitingResponse)
            {
                Logger.Error("LobbyPlayerViewModel.ChangeNameAsync: wait for a response from the server.");
                return;
            }
            
            _isWaitingResponse = true;
            await _userServerInteraction.ChangeMyNameAsync(newName);
            _isWaitingResponse = false;
        }
    }
}