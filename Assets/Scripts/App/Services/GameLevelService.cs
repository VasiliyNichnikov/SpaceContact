using App.Data;
using Client;
using Logs;
using Unity.Netcode;
using UnityEngine.SceneManagement;

namespace App.Services
{
    public class GameLevelService : IGameLevelControl
    {
        private readonly NetworkManager _networkManager;
        private readonly ScenesData _scenesData;

        private bool _isDownloadRunning;

        public GameLevelService(
            NetworkManager networkManager,
            ScenesData scenesData)
        {
            _networkManager = networkManager;
            _scenesData = scenesData;
        }

        void IGameLevelControl.StartGame()
        {
            if (_isDownloadRunning)
            {
                Logger.Error("GameLevelService.StartGame: the download is already running.");
                
                return;
            }
            
            if (!_networkManager.IsServer)
            {
                Logger.Error("GameLevelService.StartGame: only server can start buttle.");
                
                return;
            }

            _isDownloadRunning = true;
            _networkManager.SceneManager.LoadScene(_scenesData.GameSceneName, LoadSceneMode.Single);
        }
    }
}