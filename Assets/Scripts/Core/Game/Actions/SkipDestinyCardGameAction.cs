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
        [JsonConstructor]
        public SkipDestinyCardGameAction(
            ulong newTargetPlayerId, 
            long expectedPhaseRevision,
            int expectedDestinyDeckCount)
        {
            NewTargetPlayerId = newTargetPlayerId;
            ExpectedPhaseRevision = expectedPhaseRevision;
            ExpectedDestinyDeckCount = expectedDestinyDeckCount;
        }

        public enum Status
        {
            InvalidPhase = 0,
            MissingAggressor = 1,
            InvalidActor = 2,
            TargetAlreadySelected = 3,
            InvalidOwnColorCard = 4,
            DeckExhausted = 5,
            StalePhase = 6,
            StaleDestinyCard = 7
        }

        [JsonProperty] public ulong NewTargetPlayerId { get; }

        [JsonProperty(Required = Required.Always)]
        public long ExpectedPhaseRevision { get; }

        [JsonProperty(Required = Required.Always)]
        public int ExpectedDestinyDeckCount { get; }

        public override IActionValidator GetValidator()
        {
            return new Validator(NewTargetPlayerId, ExpectedPhaseRevision, ExpectedDestinyDeckCount);
        }

        protected override IReadOnlyList<IGameEvent> ExecuteCore(GameContext context, GameActor actor)
        {
            return DestinyCardEventBuilder.Create(context, true);
        }

        private sealed class Validator : IActionValidator
        {
            private readonly ulong _newTargetPlayerId;
            private readonly long _expectedPhaseRevision;
            private readonly int _expectedDestinyDeckCount;

            public Validator(ulong newTargetPlayerId, long expectedPhaseRevision, int expectedDestinyDeckCount)
            {
                _newTargetPlayerId = newTargetPlayerId;
                _expectedPhaseRevision = expectedPhaseRevision;
                _expectedDestinyDeckCount = expectedDestinyDeckCount;
            }

            public ValidationResult Check(GameContext context, GameActor actor)
            {
                if (context.Phase.Type != GamePhaseType.Destiny)
                {
                    return ValidationResult.Failed((int)Status.InvalidPhase,
                        "Destiny cards can only be skipped during Destiny.");
                }

                if (context.Phase.Revision != _expectedPhaseRevision)
                {
                    return ValidationResult.Failed((int)Status.StalePhase,
                        "The skip command belongs to another Destiny phase entry.");
                }

                if (context.Cards.DestinyDeckCount != _expectedDestinyDeckCount)
                {
                    return ValidationResult.Failed((int)Status.StaleDestinyCard,
                        "The active Destiny card has changed since this skip command was created.");
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
