using System;
using System.Collections.Generic;
using Core.EngineData;
using Core.Game.Cards;
using Core.Game.Dto.States.Cards;
using Core.Game.Hands;
using Core.Game.Planets;
using Core.Game.Players.Visitors;
using Core.User;

namespace Core.Game.Players
{
    public class SelfGamePlayer : IGamePlayer
    {
        private readonly GamePlayerHandController _handController;
        
        public SelfGamePlayer(
            IUser user, 
            SpaceCardFactory spaceCardFactory,
            Color playerColor,
            int order)
        {
            PlayerId = user.ClientId;
            IsOwner = user.IsCurrentPlayer;
            Color = playerColor;
            Order = order;
            PlayerName = user.Name;
            _handController = new GamePlayerHandController(spaceCardFactory);
        }
        
        public ulong PlayerId { get; }
        
        public string PlayerName { get; }

        public int Order { get; }

        public Color Color { get; }

        public bool IsOwner { get; }

        public IGamePlayerHandController HandController => 
            _handController;

        public IReadOnlyCollection<IPlanet> Planets { get; private set; } = 
            Array.Empty<IPlanet>();

        public void Apply(IGamePlayerVisitor visitor) => 
            visitor.Visit(this);
        
        public void LoadStartingPlanets(IReadOnlyCollection<IPlanet> planets)
        {
            Planets = planets;
        }
        
        public void UpdateHandState(PlayerHandStateData handState) => 
            _handController.UpdateState(handState);
    }
}