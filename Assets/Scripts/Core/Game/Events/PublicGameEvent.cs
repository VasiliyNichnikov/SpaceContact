using System.Collections.Generic;
using Core.Game.Visibility;

namespace Core.Game.Events
{
    public abstract class PublicGameEvent : IGameEvent
    {
        public abstract void Apply(GameContext context);

        public IEnumerable<IGameEvent> ToClientEvents(GameContext context, ulong recipientPlayerId)
        {
            StateProjection.EnsurePublic(this, recipientPlayerId);
            yield return this;
        }
    }
}
