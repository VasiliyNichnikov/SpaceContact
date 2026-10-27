using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Core.Game.Dto.Snapshots;
using Core.Game.Events;
using Core.Game.Validation;
using Newtonsoft.Json;

namespace Core.Game.Actions
{
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class SelectTargetPlanetGameAction : AutomaticClientGameAction
    {
        [JsonConstructor]
        public SelectTargetPlanetGameAction(int planetId, long expectedPhaseRevision, int expectedDestinyDeckCount,
            ulong? defenderPlayerId = null)
        {
            PlanetId = planetId;
            DefenderPlayerId = defenderPlayerId;
            ExpectedPhaseRevision = expectedPhaseRevision;
            ExpectedDestinyDeckCount = expectedDestinyDeckCount;
        }

        public enum Status
        {
            InvalidPhase = 0,
            MissingAggressor = 1,
            InvalidActor = 2,
            TargetAlreadySelected = 3,
            InvalidDestinyCard = 4,
            InvalidPlanet = 5,
            InvalidOwnSystemTarget = 6,
            InvalidForeignSystemTarget = 7,
            InvalidDefenderSelection = 8,
            StalePhase = 9,
            StaleDestinyCard = 10
        }

        [JsonProperty] [StateRequired] public int PlanetId { get; }

        [JsonProperty] public ulong? DefenderPlayerId { get; }

        [JsonProperty(Required = Required.Always)]
        public long ExpectedPhaseRevision { get; }

        [JsonProperty(Required = Required.Always)]
        public int ExpectedDestinyDeckCount { get; }

        public override IActionValidator GetValidator()
        {
            return new Validator(PlanetId, DefenderPlayerId, ExpectedPhaseRevision, ExpectedDestinyDeckCount);
        }

        protected override IReadOnlyList<IGameEvent> ExecuteCore(GameContext context, GameActor actor)
        {
            var defenderPlayerId = DefenderPlayerId ?? context.Galaxy.GetPlanetOwnerId(PlanetId);
            var events = new List<IGameEvent>();
            if (!context.Encounter.HasDefender)
            {
                events.Add(new ApplyDefenderPlayerGameEvent(defenderPlayerId));
            }

            events.Add(new ApplyPlanetToAttackGameEvent(PlanetId));
            return events;
        }

        private sealed class Validator : IActionValidator
        {
            private readonly int _planetId;
            private readonly ulong? _defenderPlayerId;
            private readonly long _expectedPhaseRevision;
            private readonly int _expectedDestinyDeckCount;

            public Validator(int planetId, ulong? defenderPlayerId, long expectedPhaseRevision,
                int expectedDestinyDeckCount)
            {
                _planetId = planetId;
                _defenderPlayerId = defenderPlayerId;
                _expectedPhaseRevision = expectedPhaseRevision;
                _expectedDestinyDeckCount = expectedDestinyDeckCount;
            }

            public ValidationResult Check(GameContext context, GameActor actor)
            {
                if (!CheckPhase(context, out var error))
                {
                    return ValidationResult.Failed((int)Status.InvalidPhase, error);
                }

                if (context.Phase.Revision != _expectedPhaseRevision)
                {
                    return ValidationResult.Failed((int)Status.StalePhase,
                        "The target selection belongs to another Destiny phase entry.");
                }

                if (context.Cards.DestinyDeckCount != _expectedDestinyDeckCount)
                {
                    return ValidationResult.Failed((int)Status.StaleDestinyCard,
                        "The active Destiny card has changed since this target selection was created.");
                }

                if (!CheckAggressorExists(context, out var aggressorPlayerId, out error))
                {
                    return ValidationResult.Failed((int)Status.MissingAggressor, error);
                }

                if (!CheckActor(actor, aggressorPlayerId, out error))
                {
                    return ValidationResult.Failed((int)Status.InvalidActor, error);
                }

                if (!CheckTargetNotSelected(context.Encounter, out error))
                {
                    return ValidationResult.Failed((int)Status.TargetAlreadySelected,
                        error);
                }

                if (!CheckDestinyCard(context, out var isJoker, out var cardPlayerId, out error))
                {
                    return ValidationResult.Failed((int)Status.InvalidDestinyCard, error);
                }

                if (!CheckPlanet(context, cardPlayerId, out var planetOwnerId, out error))
                {
                    return ValidationResult.Failed((int)Status.InvalidPlanet, error);
                }

                var isOwnSystem = planetOwnerId == aggressorPlayerId;
                var defenderPlayerId = _defenderPlayerId ?? planetOwnerId;
                if (isOwnSystem && !CheckOwnSystem(context, isJoker, aggressorPlayerId, out error))
                {
                    return ValidationResult.Failed((int)Status.InvalidOwnSystemTarget,
                        error);
                }

                if (!isOwnSystem && !CheckForeignSystem(planetOwnerId, out error))
                {
                    return ValidationResult.Failed((int)Status.InvalidForeignSystemTarget,
                        error);
                }

                if (!CheckDefenderSelection(context.Encounter, isJoker || isOwnSystem, defenderPlayerId, out error))
                {
                    return ValidationResult.Failed((int)Status.InvalidDefenderSelection,
                        error);
                }

                return ValidationResult.Ok;
            }

            private static bool CheckPhase(GameContext context, [NotNullWhen(false)] out string? error)
            {
                error = context.Phase.Type != GamePhaseType.Destiny
                    ? "Targets can only be selected during Destiny."
                    : null;

                return error is null;
            }

            private static bool CheckAggressorExists(GameContext context, out ulong aggressorPlayerId,
                [NotNullWhen(false)] out string? error)
            {
                var selectedAggressorId = context.Encounter.AggressorPlayerId;
                aggressorPlayerId = selectedAggressorId.GetValueOrDefault();
                error = !selectedAggressorId.HasValue || !context.Players.IsPlayerExist(aggressorPlayerId)
                    ? "The encounter requires an existing aggressor."
                    : null;

                return error is null;
            }

            private static bool CheckActor(GameActor actor, ulong aggressorPlayerId,
                [NotNullWhen(false)] out string? error)
            {
                error = actor.PlayerId != aggressorPlayerId
                    ? "Only the aggressor can select the encounter target."
                    : null;

                return error is null;
            }

            private static bool CheckTargetNotSelected(EncounterContext encounter,
                [NotNullWhen(false)] out string? error)
            {
                error = encounter.HasPlanetToAttack ? "The encounter target has already been selected." : null;

                return error is null;
            }

            private static bool CheckDestinyCard(GameContext context, out bool isJoker, out ulong? cardPlayerId,
                [NotNullWhen(false)] out string? error)
            {
                var destinyCard = context.Cards.ActiveDestinyCard;
                isJoker = destinyCard?.IsJoker ?? false;
                cardPlayerId = destinyCard?.SelectedPlayerId;
                if (destinyCard is null)
                {
                    error = "DestinyCard is null.";
                    return false;
                }

                if (destinyCard.SpecificCardId.HasValue || isJoker == cardPlayerId.HasValue)
                {
                    error = "Only Joker and color Destiny cards with exactly one form are supported.";
                    return false;
                }

                error = cardPlayerId.HasValue && !context.Players.IsPlayerExist(cardPlayerId.Value)
                    ? "The Destiny card targets an unknown player."
                    : null;

                return error is null;
            }

            private static bool CheckDefenderSelection(EncounterContext encounter, bool requiresNewDefender,
                ulong defenderPlayerId, [NotNullWhen(false)] out string? error)
            {
                if (requiresNewDefender)
                {
                    error = encounter.HasDefender ? "The defender has already been selected." : null;
                    return error is null;
                }

                error = encounter.DefenderPlayerId != defenderPlayerId
                    ? "The defender must match the color Destiny card."
                    : null;

                return error is null;
            }

            private bool CheckPlanet(GameContext context, ulong? cardPlayerId, out ulong planetOwnerId,
                [NotNullWhen(false)] out string? error)
            {
                planetOwnerId = 0;
                var planet = context.Galaxy.Planets.FirstOrDefault(candidate => candidate.Id == _planetId);
                if (planet is null)
                {
                    error = $"Planet with id {_planetId} not found in Galaxy.";
                    return false;
                }

                planetOwnerId = context.Galaxy.GetPlanetOwnerId(planet.Id);
                if (!context.Players.IsPlayerExist(planetOwnerId))
                {
                    error = "The target system belongs to an unknown player.";
                    return false;
                }

                error = cardPlayerId.HasValue && cardPlayerId.Value != planetOwnerId
                    ? "The planet must belong to the system selected by the Destiny card."
                    : null;

                return error is null;
            }

            private bool CheckOwnSystem(GameContext context, bool isJoker, ulong aggressorPlayerId,
                [NotNullWhen(false)] out string? error)
            {
                if (isJoker)
                {
                    error = "Joker can only target another player's system.";
                    return false;
                }

                if (!_defenderPlayerId.HasValue || _defenderPlayerId.Value == aggressorPlayerId ||
                    !context.Players.IsPlayerExist(_defenderPlayerId.Value))
                {
                    error = "Select an existing opponent as the defender in your own system.";
                    return false;
                }

                error = !context.Galaxy.HasPlayerShipsOnPlanet(_defenderPlayerId.Value, _planetId)
                    ? "The selected defender must have ships on the selected planet."
                    : null;

                return error is null;
            }

            private bool CheckForeignSystem(ulong planetOwnerId, [NotNullWhen(false)] out string? error)
            {
                error = _defenderPlayerId.HasValue && _defenderPlayerId.Value != planetOwnerId
                    ? "The defender must own the selected system."
                    : null;
                return error is null;
            }
        }
    }
}
