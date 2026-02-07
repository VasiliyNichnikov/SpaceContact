using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using Logger = Logs.Logger;

namespace Client.UI.Dialogs
{
    public class SimpleConnectionDialog : MonoBehaviour
    {
        [SerializeField]
        private Button _hostButton = null!;
        [SerializeField]
        private Button _clientButton = null!;
        
        [SerializeField]
        private NetworkManager _networkManager = null!;

        private void Start()
        {
            _hostButton.onClick.AddListener(() =>
            {
                Logger.Log("Host button clicked.");
                _networkManager.StartHost();
                HideUi();
            });
            
            _clientButton.onClick.AddListener(() =>
            {
                Logger.Log("Client Button clicked.");
                _networkManager.StartClient();
                HideUi();
            });
        }

        private void HideUi()
        {
            gameObject.SetActive(false);
        }
    }
}