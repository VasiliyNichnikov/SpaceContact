using System;
using System.Collections.Generic;
using Core.Game.Events;
using Newtonsoft.Json;

namespace Core.Game.Actions
{
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class SkipDestinyCardGameAction : BaseGameAction
    {
        public SkipDestinyCardGameAction(ulong newTargetPlayerId)
        {
            NewTargetPlayerId = newTargetPlayerId;
        }
        
        [JsonProperty]
        public ulong NewTargetPlayerId { get; }
        
        public override GameActionResult Execute(GameContext context, GameActor actor)
        {
            var destinyCardData = context.Cards.ActiveDestinyCard;

            if (destinyCardData == null)
            {
                return GameActionResult.Failed("The original destiny card is not selected.");
            }

            if (destinyCardData.IsJoker)
            {
                return GameActionResult.Failed("You can't skip the Joker card.");
            }

            var targetPlayerId = destinyCardData.SelectedPlayerId;

            if (targetPlayerId == null)
            {
                return GameActionResult.Failed("TargetPlayerId is null.");
            }

            var aggressorPlayerId = context.Encounter.AggressorPlayerId;

            if (aggressorPlayerId == null)
            {
                return GameActionResult.Failed("AggressorPlayerId is null.");
            }

            if (targetPlayerId != aggressorPlayerId || targetPlayerId != NewTargetPlayerId)
            {
                return GameActionResult.Failed($"Mismatch of player IDs. TargetPlayerId: {targetPlayerId}; DefenderPlayerId: {NewTargetPlayerId}.");
            }

            var nextDestinyCard = context.Cards.ViewFirstDestinyCard();

            if (nextDestinyCard == null)
            {
                return GameActionResult.Failed("Next destiny card is null.");
            }
            
            var events = new List<IGameEvent>
            {
                new ApplyFirstDestinyCardGameEvent()
            };

            if (nextDestinyCard.SelectedPlayerId != null)
            {
                events.Add(new ApplyDefenderPlayerGameEvent(nextDestinyCard.SelectedPlayerId.Value));
            }
            
            
            return GameActionResult.Success(events);
        }
    }
}