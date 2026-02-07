using UnityEngine;
using VContainer;

namespace App.Configs
{
    [CreateAssetMenu(fileName = "MainConfigs", menuName = "Configs/MainConfigs")]
    public class AppConfigs : ScriptableObject
    {
        [SerializeField]
        private ScenesConfig _scenesConfig = null!;
        
        public void Build(IContainerBuilder builder)
        {
            builder.RegisterInstance(_scenesConfig.BuildData());
        }
    }
}