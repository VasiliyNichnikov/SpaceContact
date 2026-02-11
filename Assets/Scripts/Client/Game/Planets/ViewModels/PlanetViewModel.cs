using Core.Game;
using Core.Game.Planets;
using CoreConvertor;
using UnityEngine;

namespace Client.Game.Planets.ViewModels
{
    public class PlanetViewModel
    {
        public PlanetViewModel(GamePlayer player, IPlanet planet)
        {
            PlanetColor = ColorConvertor.FromCoreColor(player.PlayerColor);
        }
        
        public Color PlanetColor { get; }
    }
}