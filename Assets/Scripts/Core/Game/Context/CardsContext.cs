using System.Collections.Generic;
using System.Linq;
using System;
using Core.Game.Dto.Snapshots;

namespace Core.Game
{
    public sealed class CardsContext : IContext<CardsSnapshot>
    {
        private List<SpaceCardData>? _spaceCardsDeck;
        private List<SpaceCardData> _spaceCardsDiscard = new();

        private List<DestinyCardData>? _destinyCardsDeck;
        private List<DestinyCardData> _destinyCardsDiscard = new();

        private Dictionary<ulong, PlayerHandData> _handsByPlayerId = new();

        public event Action? OnDestinyCardChanged;

        public DestinyCardData? ActiveDestinyCard { get; private set; }

        public bool IsSpaceDeckKnown => _spaceCardsDeck is not null;

        public bool IsDestinyDeckKnown => _destinyCardsDeck is not null;

        public PlayerHandData GetHand(ulong playerId)
        {
            return _handsByPlayerId[playerId];
        }

        public bool TryGetHand(ulong playerId, out PlayerHandData? hand)
        {
            return _handsByPlayerId.TryGetValue(playerId, out hand);
        }

        public DestinyCardData? ViewFirstDestinyCard()
        {
            var deck = GetKnownDestinyDeck();
            return deck.Count == 0 ? null : deck[0];
        }

        public void RemoveFirstDestinyCard()
        {
            GetKnownDestinyDeck().RemoveAt(0);
        }

        public void DiscardActiveDestinyCard()
        {
            if (ActiveDestinyCard is null)
            {
                return;
            }

            _destinyCardsDiscard.Add(ActiveDestinyCard);
            ActiveDestinyCard = null;
            OnDestinyCardChanged?.Invoke();
        }

        public void AddCardToPlayerHand(ulong playerId, SpaceCardData card)
        {
            _handsByPlayerId[playerId].Add(card);
        }

        public void SetPlayerHand(ulong playerId, PlayerHandData hand)
        {
            _handsByPlayerId[playerId] = hand;
        }

        public void SetActiveDestinyCard(DestinyCardData card)
        {
            ActiveDestinyCard = card;
            OnDestinyCardChanged?.Invoke();
        }

        public void LoadSnapshot(CardsSnapshot snapshot)
        {
            var spaceDeck = snapshot.SpaceDeck?.Select(SpaceCardData.Convert).ToList();
            var spaceDiscard = snapshot.SpaceDiscard.Select(SpaceCardData.Convert).ToList();
            var destinyDeck = snapshot.DestinyDeck?.Select(DestinyCardData.Convert).ToList();
            var destinyDiscard = snapshot.DestinyDiscard.Select(DestinyCardData.Convert).ToList();
            var hands = new Dictionary<ulong, PlayerHandData>();

            foreach (var (playerId, handSnapshot) in snapshot.HandsByPlayerId)
            {
                hands.Add(playerId, PlayerHandData.Convert(handSnapshot));
            }

            var activeDestinyCard = snapshot.ActiveDestinyCard != null
                ? DestinyCardData.Convert(snapshot.ActiveDestinyCard.Value)
                : null;

            _spaceCardsDeck = spaceDeck;
            _spaceCardsDiscard = spaceDiscard;
            _destinyCardsDeck = destinyDeck;
            _destinyCardsDiscard = destinyDiscard;
            _handsByPlayerId = hands;
            ActiveDestinyCard = activeDestinyCard;
        }

        public CardsSnapshot ToSnapshot()
        {
            var snapshot = new CardsSnapshot
            {
                SpaceDeck = _spaceCardsDeck?.Select(card => card.ToSnapshot()).ToList(),
                SpaceDiscard = _spaceCardsDiscard.Select(card => card.ToSnapshot()).ToList(),
                DestinyDeck = _destinyCardsDeck?.Select(card => card.ToSnapshot()).ToList(),
                DestinyDiscard = _destinyCardsDiscard.Select(card => card.ToSnapshot()).ToList(),
                ActiveDestinyCard = ActiveDestinyCard?.ToSnapshot()
            };

            foreach (var (playerId, hand) in _handsByPlayerId)
            {
                snapshot.HandsByPlayerId.Add(playerId, hand.ToSnapshot());
            }

            return snapshot;
        }

        internal void ReplaceState(CardsContext prepared)
        {
            _spaceCardsDeck = prepared._spaceCardsDeck;
            _spaceCardsDiscard = prepared._spaceCardsDiscard;
            _destinyCardsDeck = prepared._destinyCardsDeck;
            _destinyCardsDiscard = prepared._destinyCardsDiscard;
            _handsByPlayerId = prepared._handsByPlayerId;
            ActiveDestinyCard = prepared.ActiveDestinyCard;
        }

        private List<DestinyCardData> GetKnownDestinyDeck()
        {
            return _destinyCardsDeck
                ?? throw new InvalidOperationException("The Destiny deck is hidden in this context.");
        }
    }
}
