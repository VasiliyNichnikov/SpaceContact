using Client.Configs.Game;
using VContainer;
using VContainer.Unity;

namespace Client.UI.Loaders
{
    public class GameUILoader
    {
        private readonly IObjectResolver _resolver;
        private readonly HUDsRegistrySO _hudsRegistrySO;
        private readonly SceneStorage _sceneStorage;
        
        public GameUILoader(
            IObjectResolver resolver, 
            HUDsRegistrySO hudsRegistrySO,
            SceneStorage sceneStorage)
        {
            _resolver = resolver;
            _hudsRegistrySO = hudsRegistrySO;
            _sceneStorage = sceneStorage;
        }
        
        public void Load()
        {
            var prefab = _hudsRegistrySO.GameHud;
            var gameHud = _resolver.Instantiate(prefab, _sceneStorage.GuiHolder);
            gameHud.Init();
        }
    }
}