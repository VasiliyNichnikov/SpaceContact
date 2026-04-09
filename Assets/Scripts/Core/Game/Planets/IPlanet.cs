using System.Collections.Generic;

namespace Core.Game.Planets
{
    public interface IPlanet
    {
        int Id { get; }
        
        ulong OwnerId { get; }
        
        IReadOnlyCollection<ISpaceShip> Ships { get; }
    }
}