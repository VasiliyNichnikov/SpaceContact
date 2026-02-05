using UnityEngine;

namespace Client
{
    /// <summary>
    /// TODO: сюда надо прокинуть объекты, которые будут жить на протяжение всей игры
    /// И не будут удаляться со сцены.
    /// </summary>
    public class SceneStorage : MonoBehaviour
    {
        [SerializeField]
        private RectTransform _guiHolder = null!;
        
        public RectTransform GuiHolder => _guiHolder;
    }
}