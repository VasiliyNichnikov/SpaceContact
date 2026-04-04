using Client.UI.Dialogs.Lobby.ViewModels;
using Client.UI.Extensions;
using Reactivity;
using UnityEngine;
using UnityEngine.UI;

namespace Client.UI.Dialogs.Lobby
{
    public class LobbyPlayerSlotView : MonoBehaviour
    {
        [SerializeField]
        private Image _selectionColorButtonImage = null!;
        
        [SerializeField]
        private InputField _nameInputField = null!;
        
        [SerializeField]
        private Text _playerNameText = null!;
        
        [SerializeField]
        private GameObject _changeNameButtonGameObject = null!;
        
        [SerializeField]
        private GameObject _toLeaveButtonGameObject = null!;
        
        [SerializeField]
        private LobbyColorSelectionPanel _colorSelectionPanel = null!;

        private LobbyPlayerViewModel _viewModel = null!;
        
        public void Init(LobbyPlayerViewModel viewModel)
        {
            gameObject.UpdateChildViewModel(ref _viewModel, viewModel);
            gameObject.Subscribe(_viewModel.Name, _playerNameText.SetText);
            gameObject.Subscribe(_viewModel.Color, color => _selectionColorButtonImage.color = color);
            _colorSelectionPanel.Init(_viewModel.ColorSelectionPanelViewModel);
            RefreshView();
        }
        
        public void Show() => 
            gameObject.SetActive(true);
        
        public void Hide() => 
            gameObject.SetActive(false);
        
        /// <summary>
        /// Called from Unity
        /// </summary>
        public void ChangeNameButtonClick() => 
            _viewModel.ChangeNameClickHandler(_nameInputField.text);

        /// <summary>
        /// Called from Unity
        /// </summary>
        public void ToLeaveLobbyButtonClick() => 
            _viewModel.ToLeaveButtonClick();
        
        /// <summary>
        /// Called from Unity
        /// </summary>
        public void OnColorSelectionButtonClick() => 
            _viewModel.OnColorSelectionButtonClickHandler();

        private void RefreshView()
        {
            _changeNameButtonGameObject.SetActive(_viewModel.IsMe);
            _toLeaveButtonGameObject.SetActive(_viewModel is { IsMe: true, IsOwnerLobby: false });
            _nameInputField.gameObject.SetActive(_viewModel.IsMe);
            _playerNameText.gameObject.SetActive(!_viewModel.IsMe);

            if (_viewModel.IsMe)
            {
                _nameInputField.text = _viewModel.Name.Value;
            }
            else
            {
                _playerNameText.SetText(_viewModel.Name.Value);
            }
        }
    }
}