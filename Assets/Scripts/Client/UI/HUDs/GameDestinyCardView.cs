using Client.UI.HUDs.ViewModels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Client.UI.HUDs
{
    public class GameDestinyCardView : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _descriptionText = null!;
        
        [SerializeField]
        private Image _background = null!;
        
        public void Refresh(IGameDestinyCardViewModel viewModel)
        {
            _background.color = viewModel.BackgroundColor;
            _descriptionText.SetText(viewModel.Description);
        }
    }
}