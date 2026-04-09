using System.Collections.Generic;

namespace Core.Game.Mutation
{
    /// <summary>
    /// Функционал доступен только серверу
    /// </summary>
    public interface IServerEventBroadcaster
    {
        void SendEvent(IServerGameEvent serverEvent, RecipientType recipientType);
        
        void SendEvent(IEnumerable<IServerGameEvent> serverEvents, RecipientType recipientType);
    }
}