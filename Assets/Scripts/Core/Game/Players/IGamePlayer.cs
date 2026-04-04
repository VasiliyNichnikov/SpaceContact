using System.Collections.Generic;
using Core.EngineData;
using Core.Game.Hands;
using Core.Game.Planets;
using Core.Game.Players.Visitors;

namespace Core.Game.Players
{
    /// <summary>
    /// Любой игрок, который есть в игре
    /// </summary>
    public interface IGamePlayer
    {
        ulong PlayerId { get; }
        
        string PlayerName { get; }
        
        /// <summary>
        /// Порядок хода
        /// </summary>
        int Order { get; }
        
        Color Color { get; }
        
        bool IsOwner { get; }
        
        IGamePlayerHandController HandController { get; }
        
        IReadOnlyCollection<IPlanet> Planets { get; }
        
        void Apply(IGamePlayerVisitor visitor);
    }
}