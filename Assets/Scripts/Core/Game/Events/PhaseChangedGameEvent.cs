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
        public PhaseChangedGameEvent(GamePhaseType nextPhaseType, long phaseRevision, double? endTime)
        {
            NextPhaseType = nextPhaseType;
            PhaseRevision = phaseRevision;
            EndTime = endTime;
        }

        [StateVisibility(StateAudience.Everyone)]
        [JsonProperty]
        public GamePhaseType NextPhaseType { get; }

        [StateVisibility(StateAudience.Everyone)]
        [JsonProperty]
        public long PhaseRevision { get; }

        [StateVisibility(StateAudience.Everyone)]
        [JsonProperty]
        public double? EndTime { get; }

        public override void Apply(GameContext context)
        {
            context.Phase.SetPhase(NextPhaseType, PhaseRevision, EndTime);
        }
    }
}
