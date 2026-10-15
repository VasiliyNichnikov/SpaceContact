using System;
using Core.Game.Visibility;
using Newtonsoft.Json;

namespace Core.Game.Events
{
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class PlayerReadyToggledGameEvent : PublicGameEvent
    {
        public PlayerReadyToggledGameEvent(ulong playerId, bool isReady)
        {
            PlayerId = playerId;
            IsReady = isReady;
        }

        [JsonProperty]
        [StateVisibility(StateAudience.Everyone, IsOwnerId = true)]
        public ulong PlayerId { get; }

        [JsonProperty]
        [StateVisibility(StateAudience.Everyone)]
        public bool IsReady { get; }

        public override void Apply(GameContext context)
        {
            context.Players.SetReadyState(PlayerId, IsReady);
        }
    }
}
