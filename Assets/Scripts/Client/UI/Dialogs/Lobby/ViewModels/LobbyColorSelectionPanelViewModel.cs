using System;
using System.Collections.Generic;
using Core.User;
using CoreConvertor;
using Reactivity;

namespace Client.UI.Dialogs.Lobby.ViewModels
{
    public class LobbyColorSelectionPanelViewModel : IDisposable
    {
        private readonly ReactivityProperty<bool> _isVisible = new();
        private readonly Dictionary<int, LobbyColorButtonViewModel> _buttons;
        
        private readonly IUsersColorProvider _colorProvider;

        public LobbyColorSelectionPanelViewModel(
            Action<int> changeColorAction, 
            IUsersColorProvider colorProvider)
        {
            _colorProvider = colorProvider;
            _buttons = CreateButtons(changeColorAction);
            
            _colorProvider.ColorsChanged += RefreshButtons;
        }

        public IReadOnlyCollection<LobbyColorButtonViewModel> Buttons => 
            _buttons.Values;

        public IReactivityProperty<bool> IsVisible => 
            _isVisible;
        
        public void OpenOrClosePanel() => 
            _isVisible.Value = !_isVisible.Value;
        
        public void Dispose()
        {
            _colorProvider.ColorsChanged -= RefreshButtons;
        }

        private void RefreshButtons()
        {
            foreach (var (color, button) in _buttons)
            {
                if (_colorProvider.IsColorAvailableForSelection(color))
                {
                    button.Unlock();
                }
                else
                {
                    button.Lock();
                }
            }
        }

        private Dictionary<int, LobbyColorButtonViewModel> CreateButtons(Action<int> changeColorAction)
        {
            var buttons = new Dictionary<int, LobbyColorButtonViewModel>();
            
            foreach (var colorId in _colorProvider.AllColorIds)
            {
                var color = _colorProvider.GetColor(colorId);
                var playerUnityColor = ColorConvertor.FromCoreColor(color);
                var isColorAvailable = _colorProvider.IsColorAvailableForSelection(colorId);
                var createdButton = new LobbyColorButtonViewModel(
                    playerUnityColor, 
                    isColorAvailable, 
                    () => changeColorAction.Invoke(colorId));
                buttons.Add(colorId, createdButton);
            }
            
            return buttons;
        }
    }
}