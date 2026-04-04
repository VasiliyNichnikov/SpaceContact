using System.Collections.Generic;
using Core.Game.Players;

namespace Core.Game
{
    public interface IGameFieldManager
    {
        IGamePlayer CurrentPlayer { get; }
        
        IReadOnlyCollection<IGamePlayer> Opponents { get; }
        
        int NumberOfPlanetsOnPlayer { get; }
    }
}