using System;
using System.Linq;
using Core.Game.Dto.Snapshots;
using Core.Game.Events;
using Newtonsoft.Json;

namespace Core.Game.Actions
{
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class StartEncounterGameAction : BaseGameAction
    {
        public override GameActionResult Execute(GameContext context, GameActor actor)
        {
            if (context.Phase.CurrentPhase is not GamePhaseType.FirstMove)
            {
                return GameActionResult.Failed("Start encounter can only be started in the phase FirstMove.");
            }
            
            var playersSortedByOrder = context
                .Players
                .All
                .OrderBy(p => p.Order)
                .ToList();

            if (playersSortedByOrder.Count == 0)
            {
                return GameActionResult.Failed("Players not found.");
            }

            var encounter = context.Encounter;
            var nextAggressorIndex = 0;

            if (encounter.HasAggressor)
            {
                var aggressorPlayerId = encounter.AggressorPlayerId!.Value;
                var currentAggressorIndex = playersSortedByOrder
                    .FindIndex(p => p.Id == aggressorPlayerId);
                nextAggressorIndex = currentAggressorIndex + 1 < playersSortedByOrder.Count
                    ? currentAggressorIndex + 1
                    : 0;
            }
            
            var selectedAggressor = playersSortedByOrder[nextAggressorIndex];
            
            return GameActionResult.Success(new ApplyAggressorPlayerGameEvent(selectedAggressor.Id));
        }
    }
}