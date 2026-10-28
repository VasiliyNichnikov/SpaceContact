using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Game
{
    public sealed class GameFieldManager : IGameFieldManager
    {
        private readonly ClientGameContext _context;
        
        public GameFieldManager(ClientGameContext context)
        {
            _context = context;
        }

        public bool IsReady => _context.LocalPlayerState is not null;

        public GamePlayerData CurrentPlayer => _context.LocalPlayerState
            ?? throw new InvalidOperationException("The field requires an initialized client context.");

        public IReadOnlyCollection<GamePlayerData> Opponents
        {
            get
            {
                if (!IsReady)
                {
                    throw new InvalidOperationException("The field requires an initialized client context.");
                }

                var players = _context
                    .Players
                    .All
                    .OrderBy(p => p.Order)
                    .Where(p => p.Id != _context.LocalPlayerId)
                    .ToList();

                return players;
            }
        }
    }
}
