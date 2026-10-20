using System;
using System.Collections.Generic;
using Core.Game.Dto.Snapshots;
using Core.Game.Events;
using Newtonsoft.Json;

namespace Core.Game.Actions
{
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class SkipDestinyCardGameAction : AutomaticClientGameAction
    {
        public SkipDestinyCardGameAction(ulong newTargetPlayerId)
        {
            NewTargetPlayerId = newTargetPlayerId;
        }
        
        public enum Status
        {
            InvalidPhase = 0,
            MissingAggressor = 1,
            InvalidActor = 2,
            TargetAlreadySelected = 3,
            InvalidOwnColorCard = 4,
            DeckExhausted = 5
        }

        [JsonProperty] public ulong NewTargetPlayerId { get; }

        public override IActionValidator GetValidator()
        {
            return new Validator(NewTargetPlayerId);
        }

        protected override IReadOnlyList<IGameEvent> ExecuteCore(GameContext context, GameActor actor)
        {
            return DestinyCardEventBuilder.Create(context, true);
        }

        private sealed class Validator : IActionValidator
        {
            private readonly ulong _newTargetPlayerId;

            public Validator(ulong newTargetPlayerId)
            {
                _newTargetPlayerId = newTargetPlayerId;
            }

            public ValidationResult Check(GameContext context, GameActor actor)
            {
                if (context.Phase.Type != GamePhaseType.Destiny)
                {
                    return ValidationResult.Failed((int)Status.InvalidPhase,
                        "Destiny cards can only be skipped during Destiny.");
                }

                var aggressorPlayerId = context.Encounter.AggressorPlayerId;
                if (!aggressorPlayerId.HasValue || !context.Players.IsPlayerExist(aggressorPlayerId.Value))
                {
                    return ValidationResult.Failed((int)Status.MissingAggressor,
                        "The encounter requires an existing aggressor.");
                }

                if (actor.PlayerId != aggressorPlayerId)
                {
                    return ValidationResult.Failed((int)Status.InvalidActor,
                        "Only the aggressor can skip a Destiny card.");
                }

                if (context.Encounter.HasPlanetToAttack)
                {
                    return ValidationResult.Failed((int)Status.TargetAlreadySelected,
                        "The encounter target has already been selected.");
                }

                var activeCard = context.Cards.ActiveDestinyCard;
                if (activeCard is null || activeCard.IsJoker || activeCard.SpecificCardId.HasValue ||
                    activeCard.SelectedPlayerId != aggressorPlayerId || _newTargetPlayerId != aggressorPlayerId)
                {
                    return ValidationResult.Failed((int)Status.InvalidOwnColorCard,
                        "Only the aggressor's own color Destiny card can be skipped.");
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