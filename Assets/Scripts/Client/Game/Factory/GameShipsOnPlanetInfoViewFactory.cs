using Client.Configs.Game;
using Client.Game.Planets;
using VContainer;
using VContainer.Unity;

namespace Client.Game.Factory
{
    public sealed class GameShipsOnPlanetInfoViewFactory
    {
        private readonly IObjectResolver _resolver;
        private readonly GameUIComponentsRegistrySO _uiComponentsRegistrySO;
        private readonly SceneStorage _sceneStorage;
        
        public GameShipsOnPlanetInfoViewFactory(
            IObjectResolver resolver,
            GameUIComponentsRegistrySO uiComponentsRegistrySO,
            SceneStorage sceneStorage)
        {
            _resolver = resolver;
            _uiComponentsRegistrySO = uiComponentsRegistrySO;
            _sceneStorage = sceneStorage;
        }
        
        public GameShipsOnPlanetInfoView Create()
        {
            var prefab = _uiComponentsRegistrySO.ShipsOnPlanetInfoView;
            
            return _resolver.Instantiate(prefab, _sceneStorage.GuiHolder);
        }
    }
}