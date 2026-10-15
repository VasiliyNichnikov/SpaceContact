using System;
using Core.Game.Visibility;
using Newtonsoft.Json;

namespace Core.Game.Dto.Snapshots
{
    [Serializable]
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public sealed class PhaseSnapshot
    {
        [StateVisibility(StateAudience.Everyone)]
        public GamePhaseType CurrentPhase = GamePhaseType.None;

        [StateVisibility(StateAudience.Everyone)]
        public double? EndTimeInSeconds;

        [StateVisibility(StateAudience.Everyone)]
        public double? ReadyTransitionTimeInSeconds;

        [StateVisibility(StateAudience.Everyone)]
        public long Revision;
    }
}
