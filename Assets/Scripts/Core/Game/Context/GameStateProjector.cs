using System;
using Core.Game.Dto.Snapshots;
using Core.Game.Visibility;

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

            return StateProjection.ToClient(_context.ToSnapshot(), recipientPlayerId);
        }
    }
}
