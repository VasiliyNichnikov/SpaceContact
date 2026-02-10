using Client.Game.Planets.ViewModels;
using Reactivity;
using UnityEngine;

namespace Client.Game.Planets
{
    public class PlanetView : MonoBehaviour
    {
        private PlanetViewModel _viewModel = null!;
        
        public void Init(PlanetViewModel viewModel)
        {
            gameObject.UpdateViewModelSimple(ref _viewModel, viewModel);
        }
    }
}