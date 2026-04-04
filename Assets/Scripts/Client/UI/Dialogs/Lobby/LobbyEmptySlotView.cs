using Client.UI.Dialogs.Lobby.ViewModels;
using UnityEngine;

namespace Client.UI.Dialogs.Lobby
{
    public class LobbyEmptySlotView : MonoBehaviour
    {
        private LobbyEmptySlotViewModel _viewModel = null!;

        public void Init(LobbyEmptySlotViewModel viewModel) => 
            _viewModel = viewModel;

        public void Show() => 
            gameObject.SetActive(true);
        
        public void Hide() => 
            gameObject.SetActive(false);
        
        /// <summary>
        /// Called from Unity
        /// </summary>
        public void OnChoosePlaceButtonClick() => 
            _viewModel.OnChoosePlaceClickHandler();
    }
}