using System;
using System.Collections.Generic;
using Core.Game.Planets;

namespace Core.Game.Players.Visitors
{
    public class GamePlayerPlanetsUpdaterVisitor : IGamePlayerVisitor
    {
        private readonly Func<ulong, IReadOnlyCollection<IPlanet>> _getPlayerPlanetsFunc;
        
        public GamePlayerPlanetsUpdaterVisitor(Func<ulong, IReadOnlyCollection<IPlanet>> getPlayerPlanetsFunc) => 
            _getPlayerPlanetsFunc = getPlayerPlanetsFunc;
        
        public void Visit(ServerGamePlayer player)
        {
            var planets = GetPlayerPlanets(player.PlayerId);
            player.LoadStartingPlanets(planets);
        }

        public void Visit(SelfGamePlayer player)
        {
            var planets = GetPlayerPlanets(player.PlayerId);
            player.LoadStartingPlanets(planets);
        }

        public void Visit(PublicGamePlayer player)
        {
            var planets = GetPlayerPlanets(player.PlayerId);
            player.LoadStartingPlanets(planets);
        }
        
        private IReadOnlyCollection<IPlanet> GetPlayerPlanets(ulong playerId) => 
            _getPlayerPlanetsFunc.Invoke(playerId);
    }
}