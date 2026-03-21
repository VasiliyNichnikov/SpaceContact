using System;
using System.Collections.Generic;
using Core.EngineData;
using Core.Game.Cards;
using Core.Game.Dto.States.Cards;
using Core.Game.Hands;
using Core.Game.Planets;
using Core.Game.Players.Visitors;
using Core.Player;

namespace Core.Game.Players
{
    public class ServerGamePlayer : IGamePlayer
    {
        private readonly GamePlayerHandController _handController;
        
        public ServerGamePlayer(IPlayerManager playerCore, SpaceCardFactory spaceCardFactory)
        {
            PlayerId = playerCore.ClientId;
            Color = playerCore.Color;
            IsOwner = playerCore.IsCurrentPlayer;
            _handController = new GamePlayerHandController(spaceCardFactory);
        }
        
        public ulong PlayerId { get; }
        
        public Color Color { get; }
        
        public bool IsOwner { get; }

        public IGamePlayerHandController HandController => 
            _handController;
        
        public PlayerHandStateData? HandState { get; private set; }

        public IReadOnlyCollection<IPlanet> Planets { get; private set; } = 
            Array.Empty<IPlanet>();

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