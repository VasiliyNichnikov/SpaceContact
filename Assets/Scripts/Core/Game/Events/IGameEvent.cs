using System.Collections.Generic;

namespace Core.Game.Events
{
    public interface IGameEvent
    {
        void Apply(GameContext context);
        
        IEnumerable<IGameEvent> ToClientEvents(GameContext context, ulong recipientPlayerId);
    }
}