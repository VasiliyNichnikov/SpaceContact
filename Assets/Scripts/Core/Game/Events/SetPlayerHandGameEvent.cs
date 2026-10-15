using System.Collections.Generic;
using System;
using Core.Game.Dto.Snapshots;
using Core.Game.Visibility;
using Newtonsoft.Json;

namespace Core.Game.Events
{
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class SetPlayerHandGameEvent : IGameEvent
    {
        public SetPlayerHandGameEvent(ulong playerId, PlayerHandSnapshot hand)
        {
            PlayerId = playerId;
            Hand = hand;
        }

        [JsonProperty]
        [StateVisibility(StateAudience.Everyone, IsOwnerId = true)]
        public ulong PlayerId { get; }

        [JsonProperty]
        [StateVisibility(StateAudience.Everyone)]
        public PlayerHandSnapshot Hand { get; }

        public void Apply(GameContext context)
        {
            context.Cards.SetPlayerHand(PlayerId, PlayerHandData.Convert(Hand));
        }

        public IEnumerable<IGameEvent> ToClientEvents(GameContext context, ulong recipientPlayerId)
        {
            yield return StateProjection.ToClient(this, recipientPlayerId);
        }
    }
}
