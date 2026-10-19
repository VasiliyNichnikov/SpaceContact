using Core.Game.Dto;

namespace Core.Game.Server
{
    public interface IServerEventBroadcaster
    {
        void SendEvent(ulong recipientPlayerId, GameEventBatchData batchData);
    }
}