using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Core.Game.Events
{
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class RemoveFirstDestinyCardGameEvent : IGameEvent
    {
        public void Apply(GameContext context)
        {
            context.Cards.RemoveFirstDestinyCard();
        }

        public IEnumerable<IGameEvent> ToClientEvents(GameContext context, ulong recipientPlayerId)
        {
            yield return new ApplyDestinyDeckCountGameEvent(context.Cards.DestinyDeckCount);
        }
    }
}
