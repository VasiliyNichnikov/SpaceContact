using System.Collections.Generic;
using System.Linq;
using System;
using Core.Game.Dto.Snapshots;

namespace Core.Game
{
    public sealed class PlayerHandData
    {
        private readonly List<SpaceCardData>? _cards;
        private readonly int _hiddenCardsCount;

        public PlayerHandData(IEnumerable<SpaceCardData> cards)
        {
            _cards = cards.ToList();
        }

        private PlayerHandData(int hiddenCardsCount)
        {
            _hiddenCardsCount = hiddenCardsCount;
        }

        public IReadOnlyList<SpaceCardData>? Cards =>
            _cards;

        public int Count =>
            _cards?.Count ?? _hiddenCardsCount;

        public bool IsKnown =>
            _cards != null;

        public static PlayerHandData Convert(PlayerHandSnapshot snapshot)
        {
            return snapshot.Cards is null
                ? new PlayerHandData(snapshot.Count)
                : new PlayerHandData(snapshot.Cards.Select(SpaceCardData.Convert));
        }

        public void Add(SpaceCardData card)
        {
            if (_cards is null)
            {
                throw new InvalidOperationException("Cards cannot be added to a hidden hand.");
            }

            _cards.Add(card);
        }

        public PlayerHandSnapshot ToSnapshot()
        {
            if (_cards == null)
            {
                return new PlayerHandSnapshot(_hiddenCardsCount);
            }

            var cards = _cards.Select(card => card.ToSnapshot()).ToList();
            
            return new PlayerHandSnapshot(cards);
        }
    }
}
