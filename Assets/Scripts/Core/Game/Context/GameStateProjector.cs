using System;
using System.Collections.Generic;
using Core.Game.Dto.Snapshots;

namespace Core.Game
{
    public sealed class GameStateProjector
    {
        private readonly GameContext _context;

        public GameStateProjector(GameContext context)
        {
            _context = context;
        }

        public long Revision =>
            _context.Revision;

        public GameSnapshot CreateSnapshot(ulong recipientPlayerId)
        {
            if (_context.Role != GameContextRole.Server || !_context.IsInitialized)
            {
                throw new InvalidOperationException("Projection requires an initialized server context.");
            }

            if (!_context.Players.IsPlayerExist(recipientPlayerId))
            {
                throw new ArgumentException("The snapshot recipient must be a participant.", nameof(recipientPlayerId));
            }

            var snapshot = _context.ToSnapshot();
            var projectedHands = new Dictionary<ulong, PlayerHandSnapshot>();

            foreach (var (playerId, hand) in snapshot.Cards.HandsByPlayerId)
            {
                projectedHands.Add(playerId, playerId == recipientPlayerId
                    ? hand
                    : new HiddenPlayerHandSnapshot(hand.Count));
            }

            snapshot.Cards.HandsByPlayerId = projectedHands;
            snapshot.Cards.SpaceDeck = null;
            snapshot.Cards.DestinyDeck = null;

            return snapshot;
        }
    }
}
