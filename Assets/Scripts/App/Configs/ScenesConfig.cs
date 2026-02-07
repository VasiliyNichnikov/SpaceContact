using App.Data;
using UnityEngine;

namespace App.Configs
{
    [CreateAssetMenu(fileName = "ScenesConfig", menuName = "Configs/App/ScenesConfig")]
    public class ScenesConfig : ScriptableObject
    {
        [SerializeField] 
        private string _menuSceneName = null!;
        
        [SerializeField] 
        private string _gameSceneName = null!;

        public ScenesData BuildData()
        {
            return new ScenesData(_menuSceneName, _gameSceneName);
        }
    }
}