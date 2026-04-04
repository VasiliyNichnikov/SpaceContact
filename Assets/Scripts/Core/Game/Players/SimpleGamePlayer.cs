using System;
using System.Collections.Generic;
using Core.EngineData;
using Core.Game.Hands;
using Core.Game.Planets;
using Core.Game.Players.Visitors;
using Core.User;

namespace Core.Game.Players
{
    public class SimpleGamePlayer : IGamePlayer
    {
        private readonly GamePlayerHiddenHandController _handController = new();
        
        public SimpleGamePlayer(
            IUser user, 
            Color playerColor)
        {
            PlayerId = user.ClientId;
            Color = playerColor;
        }
        
        public ulong PlayerId { get; }
        
        public Color Color { get; }
        
        public bool IsOwner => false;

        public IGamePlayerHandController HandController => 
            _handController;

        public IReadOnlyCollection<IPlanet> Planets { get; private set; } = 
            ArraySegment<IPlanet>.Empty;

        public void Apply(IGamePlayerVisitor visitor) => 
            visitor.Visit(this);
        
        public void LoadStartingPlanets(IReadOnlyCollection<IPlanet> planets) => 
            Planets = planets;

        public void SetNumberOfCards(int numberOfCards) => 
            _handController.UpdateState(numberOfCards);
    }
}