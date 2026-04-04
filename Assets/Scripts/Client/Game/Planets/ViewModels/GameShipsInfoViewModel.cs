using CoreConvertor;
using Color = Core.EngineData.Color;

namespace Client.Game.Planets.ViewModels
{
    public sealed class GameShipsInfoViewModel : IGameShipsOnPlanetInfoItemViewModel
    {
        public GameShipsInfoViewModel(Color playerColor, int numberShipsOnPlanet)
        {
            PlayerColor = ColorConvertor.FromCoreColor(playerColor);
            NumberShipsOnPlanetText = $"x{numberShipsOnPlanet}";
        }
        
        public UnityEngine.Color PlayerColor { get; }
        
        public string NumberShipsOnPlanetText { get; }

        public void Apply(IGameShipsOnPlanetInfoVisitor visitor) =>
            visitor.Visit(this);
    }
}