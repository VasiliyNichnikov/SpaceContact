using System;
using Reactivity;
using UnityEngine;

namespace Client.UI.Dialogs.Lobby.ViewModels
{
    public class LobbyColorButtonViewModel
    {
        private readonly ReactivityProperty<bool> _isInteractive;
        private readonly ReactivityProperty<Color> _color;
        private readonly Action _onClickAction;
        
        public LobbyColorButtonViewModel(Color color, bool isInteractive, Action onClickAction)
        {
            _color = new ReactivityProperty<Color>(color);
            _isInteractive = new ReactivityProperty<bool>(isInteractive);
            _onClickAction = onClickAction;
        }
        
        public IReactivityProperty<bool> IsInteractive => 
            _isInteractive;

        public IReactivityProperty<Color> Color => 
            _color;

        public void Lock() => 
            _isInteractive.Value = false;

        public void Unlock() => 
            _isInteractive.Value = true;

        public void OnButtonClickHandler() => 
            _onClickAction.Invoke();
    }
}