using System.Linq;
using Core.Lobby.Dto;
using CoreConvertor;
using UnityEngine;

namespace Client.Configs.Menu
{
    [CreateAssetMenu(fileName = "LobbySettings", menuName = "Configs/Menu/LobbySettings", order = 0)]
    public class LobbySettings : ScriptableObject
    {
        [SerializeField] 
        private Color[] _colorsOfPlayers = null!;

        [SerializeField, Range(1, 8)]
        private int _maxNumberOfPlayers;
        
        public LobbySettingsData BuildData()
        {
            var coreColors = _colorsOfPlayers
                .Select(ColorConvertor.FromUnityColor)
                .ToArray();
            
            return new LobbySettingsData(coreColors, _maxNumberOfPlayers);
        }
    }
}