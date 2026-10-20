using Core.Game;
using CoreConvertor;
using UnityEngine;

namespace Client.Game.Planets.ViewModels
{
    public class PlanetViewModel
    {
        public PlanetViewModel(GamePlayerData player, PlanetData planet)
        {
            PlanetColor = ColorConvertor.FromCoreColor(player.Color);
            PlanetId = planet.Id;
        }
        
        public Color PlanetColor { get; }
        
        public int PlanetId { get; }
    }
}