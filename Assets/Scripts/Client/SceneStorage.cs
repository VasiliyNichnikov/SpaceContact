using UnityEngine;

namespace Client
{
    public class SceneStorage : MonoBehaviour
    {
        [SerializeField]
        private GameObject _mainCanvasGameObject = null!;
        
        [SerializeField]
        private RectTransform _guiHolder = null!;
        
        [SerializeField]
        private GameObject _eventSystemGameObject = null!;
        
        [SerializeField]
        private GameObject _gameLifeScopeGameObject = null!;
        
        [SerializeField]
        private GameObject _networkGameController = null!;
        
        public RectTransform GuiHolder => _guiHolder;
        
        private static SceneStorage? _instance;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(this);
            }
            else
            {
                Destroy(this.gameObject);
                return;
            }
            
            DontDestroyOnLoad(_mainCanvasGameObject);
            DontDestroyOnLoad(_eventSystemGameObject);
            DontDestroyOnLoad(_gameLifeScopeGameObject);
            DontDestroyOnLoad(_networkGameController);
        }
    }
}