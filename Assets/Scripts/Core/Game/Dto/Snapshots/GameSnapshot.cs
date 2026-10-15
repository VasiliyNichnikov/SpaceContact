using System;
using Core.Game.Visibility;
using Newtonsoft.Json;

namespace Core.Game.Dto.Snapshots
{
    [Serializable]
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public sealed class GameSnapshot
    {
        [StateVisibility(StateAudience.Everyone)]
        public GamePlayersSnapshot GamePlayers = null!;

        [StateVisibility(StateAudience.Everyone)]
        public GalaxySnapshot Galaxy = null!;

        [StateVisibility(StateAudience.Everyone)]
        public CardsSnapshot Cards = null!;

        [StateVisibility(StateAudience.Everyone)]
        public EncounterSnapshot Encounter = null!;

        [StateVisibility(StateAudience.Everyone)]
        public PhaseSnapshot Phase = null!;
    }
}
