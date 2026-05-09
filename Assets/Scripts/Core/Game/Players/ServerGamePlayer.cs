using System;
using System.Collections.Generic;
using System.Linq;
using Core.EngineData;
using Core.Game.Cards;
using Core.Game.Dto.States.Cards;
using Core.Game.Hands;
using Core.Game.Planets;
using Core.Game.Players.Visitors;
using Core.User;

namespace Core.Game.Players
{
    public class ServerGamePlayer : IGamePlayer
    {
        private readonly GamePlayerHandController _handController;
        
        public ServerGamePlayer(
            IUser user, 
            SpaceCardFactory spaceCardFactory,
            Color playerColor,
            int order)
        {
            PlayerId = user.ClientId;
            Color = playerColor;
            IsOwner = user.IsCurrentPlayer;
            Order = order;
            PlayerName = user.Name;
            IsReadyToNextPhase = false;
            _handController = new GamePlayerHandController(spaceCardFactory);
        }
        
        public ulong PlayerId { get; }
        
        public string PlayerName { get; }

        public int Order { get; }

        public Color Color { get; }
        
        public bool IsOwner { get; }
        
        public bool IsReadyToNextPhase { get; set; }

        public IGamePlayerHandController HandController => 
            _handController;
        
        public PlayerHandStateData? HandState { get; private set; }

        public IReadOnlyCollection<IPlanet> Planets { get; private set; } = 
            Array.Empty<IPlanet>();

        public bool ContainsPlanet(int planetId) => 
            Planets.Any(p => p.Id == planetId);

        public void Apply(IGamePlayerVisitor visitor) => 
            visitor.Visit(this);

        public void LoadStartingPlanets(IReadOnlyCollection<IPlanet> planets) => 
            Planets = planets;

        public void UpdateHandState(PlayerHandStateData handState)
        {
            HandState = handState;
            _handController.UpdateState(handState);
        }
    }
}