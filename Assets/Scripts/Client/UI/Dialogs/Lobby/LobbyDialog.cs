using System;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using VContainer.Unity;

namespace Client.UI.Dialogs.Lobby
{
    public class LobbyDialog : BaseDialog, IStartable
    {
        [SerializeField]
        private InputField _nameInputField = null!;
        
        [SerializeField]
        private RectTransform _playersContainer = null!;

        private LobbyViewModel _viewModel = null!;
        
        [Inject]
        private void Constructor(LobbyViewModel viewModel)
        {
            _viewModel = viewModel;
        }
        
        public void Start()
        {
            _nameInputField.text = _viewModel.Name;
        }

        /// <summary>
        /// Called from Unity
        /// </summary>
        public void ChangeButtonClick()
        {
            throw new NotFiniteNumberException();
        }
    }
}