using System;
using Core.Game.Visibility;
using Newtonsoft.Json;

namespace Core.Game.Events
{
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class ApplyPlanetToAttackGameEvent : PublicGameEvent
    {
        public ApplyPlanetToAttackGameEvent(int planetId)
        {
            PlanetId = planetId;
        }

        [JsonProperty]
        [StateVisibility(StateAudience.Everyone)]
        public int PlanetId { get; }

        public override void Apply(GameContext context)
        {
            context.Encounter.SetPlanetToAttack(PlanetId);
        }
    }
}
