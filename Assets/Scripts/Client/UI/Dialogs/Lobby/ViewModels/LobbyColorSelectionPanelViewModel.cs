using System;
using System.Collections.Generic;
using Core.EngineData;
using Core.Lobby;
using CoreConvertor;
using Reactivity;

namespace Client.UI.Dialogs.Lobby.ViewModels
{
    public class LobbyColorSelectionPanelViewModel : IDisposable
    {
        private readonly ReactivityProperty<bool> _isVisible = new();
        private readonly Dictionary<Color, LobbyColorButtonViewModel> _buttons;
        
        private readonly ILobbyColorProvider _colorProvider;

        public LobbyColorSelectionPanelViewModel(
            Action<Color> onChangeColorAction, 
            ILobbyColorProvider colorProvider)
        {
            _colorProvider = colorProvider;
            _buttons = CreateButtons(onChangeColorAction);
            
            _colorProvider.OnColorsRefreshed += RefreshButtons;
        }

        public IReadOnlyCollection<LobbyColorButtonViewModel> Buttons => 
            _buttons.Values;

        public IReactivityProperty<bool> IsVisible => 
            _isVisible;
        
        public void OpenOrClosePanel() => 
            _isVisible.Value = !_isVisible.Value;
        
        public void Dispose()
        {
            _colorProvider.OnColorsRefreshed -= RefreshButtons;
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

        private Dictionary<Color, LobbyColorButtonViewModel> CreateButtons(Action<Color> onChangeColorAction)
        {
            var buttons = new Dictionary<Color, LobbyColorButtonViewModel>();
            
            foreach (var availablePlayerColor in _colorProvider.AllAvailablePlayerColors)
            {
                var playerUnityColor = ColorConvertor.FromCoreColor(availablePlayerColor);
                var isColorAvailable = _colorProvider.IsColorAvailableForSelection(availablePlayerColor);
                var createdButton = new LobbyColorButtonViewModel(
                    playerUnityColor, 
                    isColorAvailable, 
                    () => onChangeColorAction.Invoke(availablePlayerColor));
                buttons.Add(availablePlayerColor, createdButton);
            }
            
            return buttons;
        }
    }
}