using Client.Game.Planets.ViewModels;
using UnityEngine;

namespace Client.Game.Planets
{
    public class GameChoicePlanetToAttackItemView : MonoBehaviour
    {
        private GameChoicePlanetToAttackViewModel _viewModel = null!;
        
        public void Init(GameChoicePlanetToAttackViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        /// <summary>
        /// Called from Unity
        /// </summary>
        public void OnAttackPlanetButtonClick() => 
            _viewModel.OnAttackPlanetButtonClickHandler();
    }
}