using System;
using System.Collections.Generic;
using Core.EngineData;
using Core.Lobby.Dto;
using Core.Player;
using Logs;

namespace Core.Lobby
{
    public class LobbyColorProvider : ILobbyColorProvider, IDisposable
    {
        private readonly List<Color> _availableColors = new();
        
        private readonly PlayersRegistry _playersRegistry;
        
        public LobbyColorProvider(PlayersRegistry playersRegistry, LobbySettingsData data)
        {
            _playersRegistry = playersRegistry;
            _playersRegistry.OnPlayerJoined += PlayerJoined;
            _playersRegistry.OnPlayerLeft += PlayerLeft;
            AllAvailablePlayerColors = data.AllAvailablePlayerColors;
            InitStartingPlayers();
        }
        
        public event Action? OnColorsRefreshed;
        
        public IReadOnlyCollection<Color> AllAvailablePlayerColors { get; }
        
        public bool IsColorAvailableForSelection(Color color)
        {
            return _availableColors.Contains(color);
        }

        public void Dispose()
        {
            _playersRegistry.OnPlayerJoined -= PlayerJoined;
            _playersRegistry.OnPlayerLeft -= PlayerLeft;
        }

        private void InitStartingPlayers()
        {
            foreach (var player in _playersRegistry.Players)
            {
                PlayerJoined(player);
            }
        }

        private void PlayerJoined(IPlayerManager playerManager)
        {
            RecreateAvailableColors();
            
            if (_availableColors.Count == 0)
            {
                Logger.Error("LobbyColorProvider.PlayerJoined: there are not available player colors.");
                
                return;
            }
            
            var firstColor = GetFirstColorAndRemove();
            playerManager.SetColor(firstColor);
            playerManager.OnPlayerInfoUpdated += RefreshAvailableColors;
            OnColorsRefreshed?.Invoke();
        }

        private void PlayerLeft(IPlayerManager playerManager)
        {
            _availableColors.Add(playerManager.Color);
            playerManager.OnPlayerInfoUpdated -= RefreshAvailableColors;
            OnColorsRefreshed?.Invoke();
        }
        
        private void RefreshAvailableColors()
        {
            RecreateAvailableColors();
            
            OnColorsRefreshed?.Invoke();
        }
        
        private void RecreateAvailableColors()
        {
            _availableColors.Clear();
            _availableColors.AddRange(AllAvailablePlayerColors);
            
            foreach (var player in _playersRegistry.Players)
            {
                if (_availableColors.Contains(player.Color))
                {
                    _availableColors.Remove(player.Color);
                }
            }
        }
        

        private Color GetFirstColorAndRemove()
        {
            var firstColor = _availableColors[0];
            _availableColors.RemoveAt(0);
            
            return firstColor;
        }
    }
}