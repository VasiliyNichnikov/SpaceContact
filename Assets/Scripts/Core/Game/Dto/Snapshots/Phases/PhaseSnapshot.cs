using System;
using Core.Game.Validation;
using Core.Game.Visibility;
using Newtonsoft.Json;

namespace Core.Game.Dto.Snapshots
{
    [Serializable]
    [StateRule(typeof(PhaseRevisionRule))]
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public sealed class PhaseSnapshot
    {
        [StateVisibility(StateAudience.Everyone)]
        [StateRequired]
        [StateDefinedEnum]
        public GamePhaseType CurrentPhase = GamePhaseType.None;

        [StateVisibility(StateAudience.Everyone)]
        [StateRange(0, double.MaxValue)]
        public double? EndTimeInSeconds;

        [StateVisibility(StateAudience.Everyone)]
        [StateRange(0, double.MaxValue)]
        public double? ReadyTransitionTimeInSeconds;

        [StateVisibility(StateAudience.Everyone)]
        [StateRequired]
        [StateRange(0, long.MaxValue)]
        public long Revision;
    }
}
