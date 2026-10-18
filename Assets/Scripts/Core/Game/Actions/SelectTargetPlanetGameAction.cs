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
    public sealed class SelectTargetPlanetGameAction : BaseGameAction
    {
        public SelectTargetPlanetGameAction(int planetId, ulong? defenderPlayerId = null)
        {
            PlanetId = planetId;
            DefenderPlayerId = defenderPlayerId;
        }

        [JsonProperty]
        [StateRequired]
        public int PlanetId { get; }

        [JsonProperty]
        public ulong? DefenderPlayerId { get; }

        public override GameActionResult Execute(GameContext context, GameActor actor)
        {
            if (!CheckRoleAndPhase(context, out var error))
            {
                return GameActionResult.Failed(error);
            }

            if (!CheckAggressorExists(context, out var aggressorPlayerId, out error) ||
                !CheckActor(actor, aggressorPlayerId, out error) ||
                !CheckTargetNotSelected(context.Encounter, out error))
            {
                return GameActionResult.Failed(error);
            }

            if (!CheckDestinyCard(context, out var isJoker, out var cardPlayerId, out error))
            {
                return GameActionResult.Failed(error);
            }

            if (!CheckPlanet(context, cardPlayerId, out var planetOwnerId, out error))
            {
                return GameActionResult.Failed(error);
            }

            var isOwnSystem = planetOwnerId == aggressorPlayerId;
            var defenderPlayerId = DefenderPlayerId ?? planetOwnerId;
            if (isOwnSystem)
            {
                if (!CheckOwnSystem(context, isJoker, aggressorPlayerId, out error))
                {
                    return GameActionResult.Failed(error);
                }
            }
            else if (!CheckForeignSystem(planetOwnerId, out error))
            {
                return GameActionResult.Failed(error);
            }

            if (!CheckDefenderSelection(context.Encounter, isJoker || isOwnSystem, defenderPlayerId, out error))
            {
                return GameActionResult.Failed(error);
            }

            var events = new List<IGameEvent>();
            if (!context.Encounter.HasDefender)
            {
                events.Add(new ApplyDefenderPlayerGameEvent(defenderPlayerId));
            }

            events.Add(new ApplyPlanetToAttackGameEvent(PlanetId));
            return GameActionResult.Success(events);
        }

        private static bool CheckRoleAndPhase(GameContext context, [NotNullWhen(false)] out string? error)
        {
            error = context.Role != GameContextRole.Server || context.Phase.Type != GamePhaseType.Destiny
                ? "Targets can only be selected on the server during Destiny."
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
            error = !actor.IsSystem && actor.PlayerId != aggressorPlayerId
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

        private bool CheckPlanet(GameContext context,
            ulong? cardPlayerId,
            out ulong planetOwnerId,
            [NotNullWhen(false)] out string? error)
        {
            planetOwnerId = 0;
            var planet = context.Galaxy.Planets.FirstOrDefault(candidate => candidate.Id == PlanetId);
            if (planet is null)
            {
                error = $"Planet with id {PlanetId} not found in Galaxy.";
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

            if (!DefenderPlayerId.HasValue || DefenderPlayerId.Value == aggressorPlayerId ||
                !context.Players.IsPlayerExist(DefenderPlayerId.Value))
            {
                error = "Select an existing opponent as the defender in your own system.";
                return false;
            }

            error = !context.Galaxy.HasPlayerShipsOnPlanet(DefenderPlayerId.Value, PlanetId)
                ? "The selected defender must have ships on the selected planet."
                : null;

            return error is null;
        }

        private bool CheckForeignSystem(ulong planetOwnerId, [NotNullWhen(false)] out string? error)
        {
            error = DefenderPlayerId.HasValue && DefenderPlayerId.Value != planetOwnerId
                ? "The defender must own the selected system."
                : null;
            return error is null;
        }
    }
}
