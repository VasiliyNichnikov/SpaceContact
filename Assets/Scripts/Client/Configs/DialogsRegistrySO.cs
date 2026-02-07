using Client.UI.Dialogs.Lobby;
using UnityEngine;

namespace Client.Configs
{
    [CreateAssetMenu(fileName = "DialogsRegistrySO", menuName = "Configs/UI/DialogsRegistrySO", order = 0)]
    public class DialogsRegistrySO : ScriptableObject
    {
        [SerializeField]
        private LobbyDialog _lobbyDialog = null!;
        
        public LobbyDialog LobbyDialog => _lobbyDialog;
    }
}