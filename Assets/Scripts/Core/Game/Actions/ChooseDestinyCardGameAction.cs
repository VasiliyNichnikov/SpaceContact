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
            if (context.Phase.CurrentPhase != GamePhaseType.Destiny)
            {
                return GameActionResult.Failed($"The wrong phase. Current is {context.Phase.CurrentPhase}.");
            }
            
            var nextDestinyCard = context.Cards.ViewFirstDestinyCard();

            if (nextDestinyCard == null)
            {
                return GameActionResult.Failed("Next destiny card is null.");
            }
            
            var events = new List<IGameEvent> { new ApplyFirstDestinyCardGameEvent() };

            if (nextDestinyCard.SelectedPlayerId != null)
            {
                events.Add(new ApplyDefenderPlayerGameEvent(nextDestinyCard.SelectedPlayerId.Value));
            }
            
            return GameActionResult.Success(events);
        }
    }
}