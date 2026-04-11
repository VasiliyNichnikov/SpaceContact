using Client.UI.HUDs.ViewModels;
using Reactivity;
using UnityEngine;
using UnityEngine.UI;

namespace Client.UI.HUDs
{
    public class GameHudBottomSwitchButtonView : MonoBehaviour
    {
        [SerializeField]
        private Image _background = null!;

        private GameHudBottomSwitchButtonViewModel _viewModel = null!;
        
        public void Init(
            GameHudBottomSwitchButtonViewModel viewModel, 
            Color selectedColor,
            Color unselectedColor)
        {
            gameObject.UpdateChildViewModel(ref _viewModel, viewModel);
            gameObject.Subscribe(_viewModel.IsSelected, value =>
            {
                _background.color = value 
                    ? selectedColor 
                    : unselectedColor;
            });
        }

        /// <summary>
        /// Called from Unity
        /// </summary>
        public void OnButtonClick() => 
            _viewModel.OnButtonClickHandler();
    }
}