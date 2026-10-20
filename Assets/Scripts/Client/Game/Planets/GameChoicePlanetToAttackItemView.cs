using Client.Game.Planets.ViewModels;
using UnityEngine;
using TMPro;

namespace Client.Game.Planets
{
    public class GameChoicePlanetToAttackItemView : MonoBehaviour
    {
        [SerializeField] 
        private TextMeshProUGUI _attackName = null!;
        
        private GameChoicePlanetToAttackViewModel _viewModel = null!;

        public void Init(GameChoicePlanetToAttackViewModel viewModel)
        {
            _viewModel = viewModel;
            _attackName.color = viewModel.DefenderColor;
        }

        /// <summary>
        /// Called from Unity
        /// </summary>
        public void OnAttackPlanetButtonClick() =>
            _viewModel.OnAttackPlanetButtonClickHandler();
    }
}
