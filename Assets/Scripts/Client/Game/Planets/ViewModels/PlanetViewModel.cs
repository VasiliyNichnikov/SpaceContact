using Core.Game.Planets;
using Core.Game.Players;
using CoreConvertor;
using UnityEngine;

namespace Client.Game.Planets.ViewModels
{
    public class PlanetViewModel
    {
        public PlanetViewModel(IGamePlayer player, IPlanet planet)
        {
            PlanetColor = ColorConvertor.FromCoreColor(player.Color);
            PlanetId = planet.Id;
        }
        
        public Color PlanetColor { get; }
        
        public int PlanetId { get; }
    }
}