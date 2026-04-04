using Client.UI.Dialogs.Game.PlayerChoice.ViewModels;
using Reactivity;
using UnityEngine;
using VContainer;

namespace Client.UI.Dialogs.Game.PlayerChoice
{
    public class GameArrowsHolder : MonoBehaviour
    {
        [SerializeField]
        private GameObject _leftArrowGameObject = null!;
        
        [SerializeField]
        private GameObject _rightArrowGameObject = null!;

        private GameArrowsHolderViewModel _viewModel = null!;
        
        [Inject]
        public void Constructor(GameArrowsHolderViewModel viewModel)
        {
            gameObject.UpdateViewModelSimple(ref _viewModel, viewModel);
            gameObject.Subscribe(_viewModel.IsRightArrowVisible, _rightArrowGameObject.SetActive);
            gameObject.Subscribe(_viewModel.IsLeftArrowVisible, _leftArrowGameObject.SetActive);
        }

        public void Init()
        {
            // nothing
        }

        /// <summary>
        /// Called from Unity
        /// </summary>
        public void OnLeftArrowClick() => 
            _viewModel.OnLeftArrowClickHandler();

        /// <summary>
        /// Called from Unity
        /// </summary>
        public void OnRightArrowClick() => 
            _viewModel.OnRightArrowClickHandler();
    }
}