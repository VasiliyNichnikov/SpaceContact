using System;
using Core.Game.Dto.Snapshots;
using Core.Game.Visibility;
using Newtonsoft.Json;

namespace Core.Game.Events
{
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class PhaseChangedGameEvent : PublicGameEvent
    {
        public PhaseChangedGameEvent(GamePhaseType nextPhaseType, long phaseRevision)
        {
            NextPhaseType = nextPhaseType;
            PhaseRevision = phaseRevision;
        }

        [StateVisibility(StateAudience.Everyone)]
        [JsonProperty]
        public GamePhaseType NextPhaseType { get; }

        [StateVisibility(StateAudience.Everyone)]
        [JsonProperty]
        public long PhaseRevision { get; }

        public override void Apply(GameContext context)
        {
            foreach (var player in context.Players.All)
            {
                context.Players.SetReadyState(player.Id, false);
            }

            context.Phase.SetPhase(NextPhaseType, PhaseRevision);
        }
    }
}
