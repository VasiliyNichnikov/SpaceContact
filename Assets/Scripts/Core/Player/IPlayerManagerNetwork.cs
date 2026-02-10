using Core.EngineData;

namespace Core.Player
{
    public interface IPlayerManagerNetwork
    {
        void SetLocalStatus(ulong clientId, bool isOwner, bool isHost);
        
        void SyncNameFromNetwork(string oldValue, string newValue);
        
        void SyncColorFromNetwork(Color oldValue, Color newValue);
    }
}