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

        public void Add(SpaceCardData card)
        {
            _cards!.Add(card);
        }

        public static PlayerHandData Convert(PlayerHandSnapshot snapshot)
        {
            if (snapshot is KnownPlayerHandSnapshot knownHand)
            {
                return new PlayerHandData(knownHand.Cards.Select(SpaceCardData.Convert));
            }

            var hiddenHand = (HiddenPlayerHandSnapshot)snapshot;
            return new PlayerHandData(hiddenHand.CardsCount);
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
