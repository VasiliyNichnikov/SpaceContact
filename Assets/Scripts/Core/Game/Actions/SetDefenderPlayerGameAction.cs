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

            if (!context.Players.IsPlayerExist(DefenderPlayerId))
            {
                return GameActionResult.Failed($"Player with id {DefenderPlayerId} not found.");
            }

            var encounter = context.Encounter;

            if (encounter.HasDefender)
            {
                return GameActionResult.Failed("Defender is already selected.");
            }
            
            return GameActionResult.Success(new ApplyDefenderPlayerGameEvent(DefenderPlayerId));
        }
    }
}