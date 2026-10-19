using System;
using Core.Game.Dto.Snapshots;
using Newtonsoft.Json;

namespace Core.Game.Actions
{
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class SkipDestinyCardGameAction : ClientGameAction
    {
        public SkipDestinyCardGameAction(ulong newTargetPlayerId)
        {
            NewTargetPlayerId = newTargetPlayerId;
        }

        [JsonProperty]
        public ulong NewTargetPlayerId { get; }

        public override GameActionResult Execute(GameContext context, GameActor actor)
        {
            if (context.Role != GameContextRole.Server || context.Phase.Type != GamePhaseType.Destiny)
            {
                return GameActionResult.Failed("Destiny cards can only be skipped on the server during Destiny.");
            }

            var aggressorPlayerId = context.Encounter.AggressorPlayerId;
            if (!aggressorPlayerId.HasValue || !context.Players.IsPlayerExist(aggressorPlayerId.Value))
            {
                return GameActionResult.Failed("The encounter requires an existing aggressor.");
            }

            if (!actor.IsSystem && actor.PlayerId != aggressorPlayerId)
            {
                return GameActionResult.Failed("Only the aggressor can skip a Destiny card.");
            }

            if (context.Encounter.HasPlanetToAttack)
            {
                return GameActionResult.Failed("The encounter target has already been selected.");
            }

            var activeCard = context.Cards.ActiveDestinyCard;
            if (activeCard is null || activeCard.IsJoker || activeCard.SpecificCardId.HasValue ||
                activeCard.SelectedPlayerId != aggressorPlayerId || NewTargetPlayerId != aggressorPlayerId)
            {
                return GameActionResult.Failed("Only the aggressor's own color Destiny card can be skipped.");
            }

            return ChooseDestinyCardGameAction.CreateSelectionResult(context, true);
        }
    }
}
