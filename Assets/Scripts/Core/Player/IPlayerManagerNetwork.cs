using Core.EngineData;

namespace Core.Player
{
    public interface IPlayerManagerNetwork
    {
        void SetLocalStatus(bool isOwner);
        
        void SyncNameFromNetwork(string oldValue, string newValue);
        
        void SyncColorFromNetwork(Color oldValue, Color newValue);
    }
}