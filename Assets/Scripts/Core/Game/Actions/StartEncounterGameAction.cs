using System;
using System.Collections.Generic;
using System.Linq;
using Core.Game.Dto.Snapshots;
using Core.Game.Events;
using Newtonsoft.Json;

namespace Core.Game.Actions
{
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class StartEncounterGameAction : ServerGameAction
    {
        public enum Status
        {
            InvalidPhase = 0,
            MissingParticipants = 1,
            UnknownAggressor = 2
        }
        
        public override IActionValidator GetValidator()
        {
            return new Validator();
        }

        protected override IReadOnlyList<IGameEvent> ExecuteCore(GameContext context, GameActor actor)
        {
            var players = context.Players.All.OrderBy(player => player.Order).ToList();
            var aggressorPlayerId = context.Encounter.AggressorPlayerId;
            var nextIndex = aggressorPlayerId.HasValue
                ? (players.FindIndex(player => player.Id == aggressorPlayerId.Value) + 1) % players.Count
                : 0;
            return new IGameEvent[] { new ApplyAggressorPlayerGameEvent(players[nextIndex].Id) };
        }
        
        private sealed class Validator : IActionValidator
        {
            public ValidationResult Check(GameContext context, GameActor actor)
            {
                if (context.Phase.Type != GamePhaseType.FirstMove)
                {
                    return ValidationResult.Failed((int)Status.InvalidPhase,
                        "An encounter can only start during FirstMove.");
                }

                if (context.Players.All.Count == 0)
                {
                    return ValidationResult.Failed((int)Status.MissingParticipants,
                        "Players not found.");
                }

                var aggressorPlayerId = context.Encounter.AggressorPlayerId;
                if (aggressorPlayerId.HasValue && !context.Players.IsPlayerExist(aggressorPlayerId.Value))
                {
                    return ValidationResult.Failed((int)Status.UnknownAggressor,
                        "The current aggressor is not a participant.");
                }

                return ValidationResult.Ok;
            }
        }
    }
}
