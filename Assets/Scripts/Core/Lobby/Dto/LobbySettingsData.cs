using Core.EngineData;

namespace Core.Lobby.Dto
{
    public readonly struct LobbySettingsData
    {
        public readonly Color[] AllAvailablePlayerColors;

        public readonly int MaxNumberOfPlayers;
        
        public LobbySettingsData(Color[] allAvailablePlayerColors, int maxNumberOfPlayers)
        {
            AllAvailablePlayerColors = allAvailablePlayerColors;
            MaxNumberOfPlayers = maxNumberOfPlayers;
        }
    }
}