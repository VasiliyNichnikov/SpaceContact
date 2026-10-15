using System;
using Core.Game.Validation;
using Core.Game.Visibility;
using Newtonsoft.Json;

namespace Core.Game.Dto.Snapshots
{
    [Serializable]
    [StateRule(typeof(LocalPlayerSnapshotRule))]
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public sealed class GameSnapshot
    {
        [StateVisibility(StateAudience.Everyone)]
        [StateRequired]
        public GamePlayersSnapshot GamePlayers = null!;

        [StateVisibility(StateAudience.Everyone)]
        [StateRequired]
        public GalaxySnapshot Galaxy = null!;

        [StateVisibility(StateAudience.Everyone)]
        [StateRequired]
        public CardsSnapshot Cards = null!;

        [StateVisibility(StateAudience.Everyone)]
        [StateRequired]
        public EncounterSnapshot Encounter = null!;

        [StateVisibility(StateAudience.Everyone)]
        [StateRequired]
        public PhaseSnapshot Phase = null!;
    }
}
