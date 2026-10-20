using System;
using System.Collections.Generic;
using Core.Game.Dto.Snapshots;
using Core.Game.Events;
using Newtonsoft.Json;

namespace Core.Game.Actions
{
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class ChooseDestinyCardGameAction : ServerGameAction
    {
        public enum Status
        {
            InvalidPhase = 0,
            MissingAggressor = 1,
            CardAlreadyRevealed = 2,
            DeckExhausted = 3
        }
        
        public override IActionValidator GetValidator()
        {
            return new Validator();
        }

        protected override IReadOnlyList<IGameEvent> ExecuteCore(GameContext context, GameActor actor)
        {
            return DestinyCardEventBuilder.Create(context, false);
        }
        
        private sealed class Validator : IActionValidator
        {
            public ValidationResult Check(GameContext context, GameActor actor)
            {
                if (context.Phase.Type != GamePhaseType.Destiny)
                {
                    return ValidationResult.Failed((int)Status.InvalidPhase,
                        "Destiny cards can only be selected during Destiny.");
                }

                var aggressorPlayerId = context.Encounter.AggressorPlayerId;
                if (!aggressorPlayerId.HasValue || !context.Players.IsPlayerExist(aggressorPlayerId.Value))
                {
                    return ValidationResult.Failed((int)Status.MissingAggressor,
                        "The encounter requires an existing aggressor.");
                }

                if (context.Cards.ActiveDestinyCard is not null)
                {
                    return ValidationResult.Failed((int)Status.CardAlreadyRevealed,
                        "A Destiny card has already been revealed.");
                }

                if (context.Cards.DestinyDeckCount <= 0)
                {
                    return ValidationResult.Failed((int)Status.DeckExhausted,
                        "The Destiny deck is exhausted. Reshuffling is not implemented.");
                }

                return ValidationResult.Ok;
            }
        }
    }
}
