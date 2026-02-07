using Client.UI.Dialogs.Lobby;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using Logger = Logs.Logger;

namespace Client.UI.Dialogs
{
    public class SimpleConnectionDialog : MonoBehaviour
    {
        [SerializeField]
        private Button _hostButton = null!;
        [SerializeField]
        private Button _clientButton = null!;

        private ILobbyController _lobbyController = null!;
        
        [Inject]
        private void Constructor(ILobbyController lobbyController)
        {
            _lobbyController = lobbyController;
        }

        private void Start()
        {
            _hostButton.onClick.AddListener(() =>
            {
                Logger.Log("Host button clicked.");
                _lobbyController.StartHost();
                HideUi();
            });
            
            _clientButton.onClick.AddListener(() =>
            {
                Logger.Log("Client Button clicked.");
                _lobbyController.StartClient();
                HideUi();
            });
        }

        private void HideUi()
        {
            gameObject.SetActive(false);
        }
    }
}