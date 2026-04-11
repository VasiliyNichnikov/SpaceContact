using System;
using System.Linq;
using Client.UI.HUDs.ViewModels;
using UnityEngine;

namespace Client.UI.HUDs
{
    public class GameHudBottomSwitchesView : MonoBehaviour
    {
        [Serializable]
        private struct SwitchButtonViewData
        {
            public GameHudBottomSwitchButtonType Type;
            
            public GameHudBottomSwitchButtonView View;
        }
        
        [SerializeField]
        private SwitchButtonViewData[] _buttons = null!;
        
        [SerializeField]
        private Color _selectedButtonColor;
        
        [SerializeField]
        private Color _unselectedButtonColor;

        public void Init(GameHudBottomSwitchesViewModel viewModel)
        {
            var buttonViewModels = viewModel.Buttons;
            
            foreach (var buttonData in _buttons)
            {
                var buttonViewModel = buttonViewModels.First(x => x.Type == buttonData.Type);
                buttonData.View.Init(buttonViewModel, _selectedButtonColor, _unselectedButtonColor);
            }
        }
    }
}