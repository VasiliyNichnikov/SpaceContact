using System.Collections.Generic;
using Core.Game.Planets;
using Core.Player;

namespace Core.Game
{
    public class GamePlayer
    {
        private readonly IPlayerManager _core;
        
        public GamePlayer(IPlayerManager core, IReadOnlyCollection<IPlanet> planets)
        {
            _core = core;
            Planets = planets;
        }

        public ulong PlayerId => _core.ClientId;
        
        public IReadOnlyCollection<IPlanet> Planets { get; }
    }
}