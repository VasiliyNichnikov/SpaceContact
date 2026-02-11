using UnityEngine;
using VContainer;

namespace Client.Configs.Menu
{
    [CreateAssetMenu(fileName = "MenuConfigs", menuName = "Configs/Menu/MenuConfigs", order = 0)]
    public class MenuConfigs : ScriptableObject
    {
        [SerializeField]
        private LobbySettings _lobbySettings = null!;
        
        public void Build(IContainerBuilder builder)
        {
            builder.RegisterInstance(_lobbySettings.BuildData());
        }
    }
}