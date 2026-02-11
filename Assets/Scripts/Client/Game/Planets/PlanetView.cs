using Client.Game.Planets.ViewModels;
using Reactivity;
using UnityEngine;

namespace Client.Game.Planets
{
    public class PlanetView : MonoBehaviour
    {
        private static readonly int MaterialColorProperty = Shader.PropertyToID("_Color");

        [SerializeField] 
        private Renderer _renderer = null!;
        
        private PlanetViewModel _viewModel = null!;
        
        public void Init(PlanetViewModel viewModel)
        {
            gameObject.UpdateViewModelSimple(ref _viewModel, viewModel);
            ChangeColorPlanet();
        }

        private void ChangeColorPlanet()
        {
            var propBlock = new MaterialPropertyBlock();
            _renderer.GetPropertyBlock(propBlock);
            propBlock.SetColor(MaterialColorProperty, _viewModel.PlanetColor);
            _renderer.SetPropertyBlock(propBlock);
        }
    }
}