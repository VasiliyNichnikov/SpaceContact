using System.Linq;
using Client.UI.Dialogs.Lobby.ViewModels;
using Client.UI.Utils;
using Reactivity;
using UnityEngine;

namespace Client.UI.Dialogs.Lobby
{
    public class LobbyColorSelectionPanel : MonoBehaviour
    {
        [SerializeField]
        private LobbyColorButton _colorButtonPrefab = null!;
        
        [SerializeField]
        private RectTransform _colorsGrid = null!;
        
        private LobbyColorSelectionPanelViewModel _viewModel = null!;
        
        public void Init(LobbyColorSelectionPanelViewModel viewModel)
        {
            gameObject.UpdateChildViewModel(ref _viewModel, viewModel);
            gameObject.Subscribe(_viewModel.IsVisible, UpdateDisplay);
            CreateColorButtons();
        }

        private void UpdateDisplay(bool isActive)
        {
            gameObject.SetActive(isActive);
        }

        private void CreateColorButtons()
        {
            var viewModels = _viewModel.Buttons.ToList();
            UIUtils.CreateRequiredNumberOfItems(_colorsGrid, _colorButtonPrefab, viewModels, (view, viewModel) =>
            {
                view.Show();
                view.Init(viewModel);
            });
        }
    }
}