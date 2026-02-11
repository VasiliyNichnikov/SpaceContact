using Client.UI.Dialogs.Lobby.ViewModels;
using Reactivity;
using UnityEngine;
using UnityEngine.UI;

namespace Client.UI.Dialogs.Lobby
{
    public class LobbyColorButton : MonoBehaviour
    {
        [SerializeField]
        private Image _colorImage = null!;
        
        [SerializeField]
        private Button _colorButton = null!;

        private LobbyColorButtonViewModel _viewModel = null!;
        
        public void Init(LobbyColorButtonViewModel viewModel)
        {
            gameObject.UpdateViewModelSimple(ref _viewModel, viewModel);
            gameObject.Subscribe(_viewModel.Color, color => _colorImage.color = color);
            gameObject.Subscribe(_viewModel.IsInteractive, value => _colorButton.interactable = value);
        }
        
        public void Show() => 
            gameObject.SetActive(true);

        /// <summary>
        /// Called from Unity
        /// </summary>
        public void OnClick() => 
            _viewModel.OnButtonClickHandler();
    }
}