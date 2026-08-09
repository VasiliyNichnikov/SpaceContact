using System;
using System.Collections.Generic;
using Core.Game.Dto.States;
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
            if (context.Phase.Current != GamePhaseType.Destiny)
            {
                return GameActionResult.Failed($"The wrong phase. Current is {context.Phase.Current}.");
            }
            
            var destinyCardData = context.Cards.DrawDestinyCard();
            var events = new List<IGameEvent> { new ApplyDestinyCardGameEvent(destinyCardData) };

            if (destinyCardData.SelectedPlayerId != null)
            {
                events.Add(new ApplyDefenderPlayerGameEvent(destinyCardData.SelectedPlayerId.Value));
            }
            
            return GameActionResult.Success(events);
        }
    }
}