using System;
using System.Collections.Generic;
using Core.EngineData;
using Core.Game.Hands;
using Core.Game.Planets;
using Core.Game.Players.Visitors;

namespace Core.Game.Players
{
    public class EmptyGamePlayer : IGamePlayer
    {
        private static EmptyGamePlayer? _instance;
        
        private EmptyGamePlayer()
        {
            // nothing
        }

        public static IGamePlayer Instance => 
            _instance ??= new EmptyGamePlayer();
        
        public ulong PlayerId => ulong.MinValue;
        
        public int Order => int.MaxValue;

        public Color Color => default;
        
        public bool IsOwner => false;

        public IGamePlayerHandController HandController => 
            EmptyGamePlayerHandController.Instance;

        public IReadOnlyCollection<IPlanet> Planets => 
            ArraySegment<IPlanet>.Empty;

        public void Apply(IGamePlayerVisitor visitor)
        {
            // nothing
        }
    }
}