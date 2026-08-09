using System;
using Core.Game.Events;
using Newtonsoft.Json;

namespace Core.Game.Actions
{
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class SetDefenderPlayerGameAction : BaseGameAction
    {
        public SetDefenderPlayerGameAction(ulong defenderPlayerId)
        {
            DefenderPlayerId = defenderPlayerId;
        }
        
        [JsonProperty]
        public ulong DefenderPlayerId { get; }
        
        public override GameActionResult Execute(GameContext context, GameActor actor)
        {
            if (actor.PlayerId == null)
            {
                return GameActionResult.Failed("Actor player id is null.");
            }

            var encounter = context.Encounter;

            if (encounter.HasDefender)
            {
                return GameActionResult.Failed("Defender is already selected.");
            }
            
            encounter.SetDefender(DefenderPlayerId);
            return GameActionResult.Success(new ApplyDefenderPlayerGameEvent(DefenderPlayerId));
        }
    }
}