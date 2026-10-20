using System;
using System.Collections.Generic;
using Core.Game.Events;

namespace Core.Game.Actions
{
    internal static class DestinyCardEventBuilder
    {
        internal static IReadOnlyList<IGameEvent> Create(GameContext context, bool discardActiveCard)
        {
            var nextCard = context.Cards.ViewFirstDestinyCard()
                ?? throw new InvalidOperationException("The validated Destiny deck must contain a card.");
            var targetPlayerId = nextCard.SelectedPlayerId;
            var defenderPlayerId = targetPlayerId == context.Encounter.AggressorPlayerId ? null : targetPlayerId;
            var events = new List<IGameEvent>();
            if (discardActiveCard)
            {
                events.Add(new DiscardActiveDestinyCardGameEvent());
            }

            events.Add(new RemoveFirstDestinyCardGameEvent());
            events.Add(new ApplyDestinyCardGameEvent(nextCard.ToSnapshot(), defenderPlayerId));
            return events;
        }
    }
}
