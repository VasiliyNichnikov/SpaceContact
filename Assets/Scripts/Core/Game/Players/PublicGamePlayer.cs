using System;
using System.Collections.Generic;
using Core.EngineData;
using Core.Game.Hands;
using Core.Game.Planets;
using Core.Game.Players.Visitors;
using Core.User;

namespace Core.Game.Players
{
    public class PublicGamePlayer : IGamePlayer
    {
        private readonly GamePlayerHiddenHandController _handController = new();
        
        public PublicGamePlayer(
            IUser user, 
            Color playerColor,
            int order)
        {
            PlayerId = user.ClientId;
            Color = playerColor;
            Order = order;
        }
        
        public ulong PlayerId { get; }
        
        public int Order { get; }

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