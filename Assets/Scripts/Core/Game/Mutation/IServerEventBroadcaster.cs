using System.Collections.Generic;

namespace Core.Game.Mutation
{
    /// <summary>
    /// Функционал доступен только серверу
    /// </summary>
    public interface IServerEventBroadcaster
    {
        void SendEvent(IGameEventData evt, RecipientType recipientType);
        
        void SendEvent(IEnumerable<IGameEventData> evts, RecipientType recipientType);
    }
}