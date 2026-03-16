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
        }
        
        public Color PlanetColor { get; }
    }
}