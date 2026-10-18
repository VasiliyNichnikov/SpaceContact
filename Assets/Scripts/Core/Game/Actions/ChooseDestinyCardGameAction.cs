using System;
using System.Collections.Generic;
using Core.Game.Dto.Snapshots;
using Core.Game.Events;
using Newtonsoft.Json;

namespace Core.Game.Actions
{
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class ChooseDestinyCardGameAction : BaseGameAction
    {
        public override GameActionResult Execute(GameContext context, GameActor actor)
        {
            if (!actor.IsSystem)
            {
                return GameActionResult.Failed("Only the system can reveal the first Destiny card.");
            }

            if (context.Cards.ActiveDestinyCard is not null)
            {
                return GameActionResult.Failed("A Destiny card has already been revealed.");
            }

            return CreateSelectionResult(context, false);
        }

        internal static GameActionResult CreateSelectionResult(GameContext context, bool discardActiveCard)
        {
            if (context.Role != GameContextRole.Server || context.Phase.Type != GamePhaseType.Destiny)
            {
                return GameActionResult.Failed("Destiny cards can only be selected on the server during Destiny.");
            }

            var aggressorPlayerId = context.Encounter.AggressorPlayerId;
            
            if (!aggressorPlayerId.HasValue || !context.Players.IsPlayerExist(aggressorPlayerId.Value))
            {
                return GameActionResult.Failed("The encounter requires an existing aggressor.");
            }

            var nextCard = context.Cards.ViewFirstDestinyCard();
            
            if (nextCard is null)
            {
                return GameActionResult.Failed("The Destiny deck is exhausted. Reshuffling is not implemented.");
            }

            if (nextCard.SpecificCardId.HasValue || nextCard.IsJoker == nextCard.SelectedPlayerId.HasValue)
            {
                return GameActionResult.Failed("Only Joker and color Destiny cards with exactly one form are supported.");
            }

            var targetPlayerId = nextCard.SelectedPlayerId;
            if (targetPlayerId.HasValue && !context.Players.IsPlayerExist(targetPlayerId.Value))
            {
                return GameActionResult.Failed("The Destiny card targets an unknown player.");
            }

            // A color identifies a system; an enemy in the aggressor's system is selected separately.
            var defenderPlayerId = targetPlayerId == aggressorPlayerId ? null : targetPlayerId;
            var events = new List<IGameEvent>();
            if (discardActiveCard)
            {
                events.Add(new DiscardActiveDestinyCardGameEvent());
            }

            events.Add(new RemoveFirstDestinyCardGameEvent());
            events.Add(new ApplyDestinyCardGameEvent(nextCard.ToSnapshot(), defenderPlayerId));
            return GameActionResult.Success(events);
        }
    }
}
