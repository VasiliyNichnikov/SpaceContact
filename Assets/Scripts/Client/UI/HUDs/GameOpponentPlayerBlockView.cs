using Client.UI.HUDs.ViewModels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Client.UI.HUDs
{
    public class GameOpponentPlayerBlockView : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _playerNameText = null!;
        
        [SerializeField]
        private Image _background = null!;
        
        public void Refresh(GamePlayerBlockViewModel viewModel)
        {
            _playerNameText.SetText(viewModel.PlayerName);
            _background.color = viewModel.PlayerColor;
        }
    }
}