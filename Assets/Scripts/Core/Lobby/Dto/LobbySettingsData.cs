using Core.EngineData;

namespace Core.Lobby.Dto
{
    public readonly struct LobbySettingsData
    {
        public readonly Color[] AllAvailablePlayerColors;
        
        public LobbySettingsData(Color[] allAvailablePlayerColors)
        {
            AllAvailablePlayerColors = allAvailablePlayerColors;
        }
    }
}