using System;
using System.Collections.Generic;
using System.Linq;
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
            if (snapshot is KnownPlayerHandSnapshot knownHand)
            {
                return new PlayerHandData(knownHand.Cards.Select(SpaceCardData.Convert));
            }

            if (snapshot is HiddenPlayerHandSnapshot hiddenHand)
            {
                return new PlayerHandData(hiddenHand.CardsCount);
            }

            throw new ArgumentException("Unsupported hand snapshot type.", nameof(snapshot));
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
                return new HiddenPlayerHandSnapshot(_hiddenCardsCount);
            }

            return new KnownPlayerHandSnapshot(
                _cards.Select(card => card.ToSnapshot()).ToList());
        }
    }
}
