using System;
using System.Collections.Generic;
using App.Data;
using Client;
using Core.Phases;
using Logs;
using Network;
using Unity.Netcode;
using UnityEngine.SceneManagement;

namespace App.Services
{
    public class GameLevelService : IGameLevelControl, IDisposable
    {
        private readonly NetworkManager _networkManager;
        private readonly NetworkGameController _networkGameController;
        private readonly ScenesData _scenesData;

        private bool _isDownloadRunning;

        public GameLevelService(
            NetworkManager networkManager,
            NetworkGameController networkGameController,
            ScenesData scenesData)
        {
            _networkManager = networkManager;
            _networkGameController = networkGameController;
            _scenesData = scenesData;

            if (_networkManager.SceneManager != null)
            {
                _networkManager.SceneManager.OnLoadEventCompleted += OnSceneLoadCompleted;
            }
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
        
        public void Dispose()
        {
            if (_networkManager != null && _networkManager.SceneManager != null)
            {
                _networkManager.SceneManager.OnLoadEventCompleted -= OnSceneLoadCompleted;
            }
        }

        private void OnSceneLoadCompleted(
            string sceneName, 
            LoadSceneMode loadMode,
            List<ulong> clientsCompleted,
            List<ulong> clientsTimedOut)
        {
            if (!_networkManager.IsServer)
            {
                return;
            }

            if (_scenesData.GameSceneName == sceneName)
            {
                _networkGameController.ServerTransitionTo<RegroupPhase>();
            }

            _isDownloadRunning = false;
        }
    }
}