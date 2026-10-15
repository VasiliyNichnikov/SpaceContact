using System;
using Core.Game.Visibility;
using Newtonsoft.Json;

namespace Core.Game.Events
{
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class ApplyDefenderPlayerGameEvent : PublicGameEvent
    {
        public ApplyDefenderPlayerGameEvent(ulong defenderPlayerId)
        {
            DefenderPlayerId = defenderPlayerId;
        }

        [JsonProperty]
        [StateVisibility(StateAudience.Everyone)]
        public ulong DefenderPlayerId { get; }

        public override void Apply(GameContext context)
        {
            context.Encounter.SetDefender(DefenderPlayerId);
        }
    }
}
