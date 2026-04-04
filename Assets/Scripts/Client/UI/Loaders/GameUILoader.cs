using Client.Configs.Game;
using VContainer;
using VContainer.Unity;

namespace Client.UI.Loaders
{
    public class GameUILoader
    {
        private readonly IObjectResolver _resolver;
        private readonly HUDsRegistrySO _hudsRegistrySO;
        private readonly GameUIComponentsRegistrySO _uiComponentsRegistrySO;
        private readonly SceneStorage _sceneStorage;
        
        public GameUILoader(
            IObjectResolver resolver, 
            HUDsRegistrySO hudsRegistrySO,
            GameUIComponentsRegistrySO uiComponentsRegistrySO,
            SceneStorage sceneStorage)
        {
            _resolver = resolver;
            _hudsRegistrySO = hudsRegistrySO;
            _uiComponentsRegistrySO = uiComponentsRegistrySO;
            _sceneStorage = sceneStorage;
        }
        
        public void Load()
        {
            LoadGameHud();
            LoadArrowsHolder();
        }

        private void LoadGameHud()
        {
            var prefab = _hudsRegistrySO.GameHud;
            var gameHud = _resolver.Instantiate(prefab, _sceneStorage.GuiHolder);
            gameHud.Init();
        }

        private void LoadArrowsHolder()
        {
            var prefab = _uiComponentsRegistrySO.ArrowsHolder;
            var arrowsHolder = _resolver.Instantiate(prefab, _sceneStorage.GuiHolder);
            arrowsHolder.Init();
        }
    }
}