using Client.Game.Planets.ViewModels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Client.Game.Planets
{
    public class GameShipsInfoItemView : MonoBehaviour
    {
        [SerializeField] 
        private Image _playerColorImage = null!;
        
        [SerializeField]
        private TextMeshProUGUI _numberOfShipsText = null!;

        public void Init(GameShipsInfoViewModel viewModel)
        {
            _playerColorImage.color = viewModel.PlayerColor;
            _numberOfShipsText.SetText(viewModel.NumberShipsOnPlanetText);
        }
    }
}