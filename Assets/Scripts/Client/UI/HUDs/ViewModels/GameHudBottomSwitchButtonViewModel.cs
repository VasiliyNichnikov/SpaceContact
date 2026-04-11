using System;
using Reactivity;

namespace Client.UI.HUDs.ViewModels
{
    public sealed class GameHudBottomSwitchButtonViewModel
    {
        private readonly ReactivityProperty<bool> _isSelected = new();
        private readonly Action _onClickHandler;
        
        public GameHudBottomSwitchButtonViewModel(GamePlayerInfoTabType type, Action onClickHandler)
        {
            Type = type;
            _onClickHandler = onClickHandler;
        }

        public IReactivityProperty<bool> IsSelected => 
            _isSelected;
        
        public GamePlayerInfoTabType Type { get; }

        public void Select() => 
            _isSelected.Value = true;
        
        public void Deselect() => 
            _isSelected.Value = false;
        
        public void OnButtonClickHandler() => 
            _onClickHandler.Invoke();
    }
}