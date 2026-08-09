using System;
using System.Collections.Generic;
using System.Linq;
using Core.Game.Dto.Snapshots;

namespace Core.Game
{
    public sealed class CardsContext : IContext<CardsSnapshot>
    {
        private readonly List<SpaceCardData> _spaceCardsDeck = new();
        private readonly List<SpaceCardData> _spaceCardsDiscard = new();

        private readonly List<DestinyCardData> _destinyCardsDeck = new();
        private readonly List<DestinyCardData> _destinyCardsDiscard = new();

        private readonly Dictionary<ulong, PlayerHandData> _handsByPlayerId = new();

        public event Action? OnDestinyCardChanged;

        public DestinyCardData? ActiveDestinyCard { get; private set; }

        public PlayerHandData GetHand(ulong playerId)
        {
            return _handsByPlayerId[playerId];
        }

        public bool TryGetHand(ulong playerId, out PlayerHandData hand)
        {
            return _handsByPlayerId.TryGetValue(playerId, out hand);
        }

        public DestinyCardData? ViewFirstDestinyCard()
        {
            return _destinyCardsDeck.Count == 0 ? null : _destinyCardsDeck[0];
        }

        public DestinyCardData DrawDestinyCard()
        {
            var card = _destinyCardsDeck[0];
            _destinyCardsDeck.RemoveAt(0);
            _destinyCardsDiscard.Add(card);
            return card;
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
            _spaceCardsDeck.Clear();
            _spaceCardsDiscard.Clear();

            _destinyCardsDeck.Clear();
            _destinyCardsDiscard.Clear();
            _handsByPlayerId.Clear();

            _spaceCardsDeck.AddRange(snapshot.SpaceDeck.Select(SpaceCardData.Convert));
            _spaceCardsDiscard.AddRange(snapshot.SpaceDiscard.Select(SpaceCardData.Convert));

            _destinyCardsDeck.AddRange(snapshot.DestinyDeck.Select(DestinyCardData.Convert));
            _destinyCardsDiscard.AddRange(snapshot.DestinyDiscard.Select(DestinyCardData.Convert));

            foreach (var (playerId, handSnapshot) in snapshot.HandsByPlayerId)
            {
                _handsByPlayerId.Add(playerId, PlayerHandData.Convert(handSnapshot));
            }

            ActiveDestinyCard = snapshot.ActiveDestinyCard != null
                ? DestinyCardData.Convert(snapshot.ActiveDestinyCard.Value)
                : null;
        }

        public CardsSnapshot ToSnapshot()
        {
            var snapshot = new CardsSnapshot
            {
                SpaceDeck = _spaceCardsDeck.Select(card => card.ToSnapshot()).ToList(),
                SpaceDiscard = _spaceCardsDiscard.Select(card => card.ToSnapshot()).ToList(),
                DestinyDeck = _destinyCardsDeck.Select(card => card.ToSnapshot()).ToList(),
                DestinyDiscard = _destinyCardsDiscard.Select(card => card.ToSnapshot()).ToList(),
                ActiveDestinyCard = ActiveDestinyCard?.ToSnapshot()
            };

            foreach (var (playerId, hand) in _handsByPlayerId)
            {
                snapshot.HandsByPlayerId.Add(playerId, hand.ToSnapshot());
            }

            return snapshot;
        }
    }
}
