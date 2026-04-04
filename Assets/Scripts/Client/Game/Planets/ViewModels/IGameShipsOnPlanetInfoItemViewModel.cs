namespace Client.Game.Planets.ViewModels
{
    public interface IGameShipsOnPlanetInfoItemViewModel
    {
        void Apply(IGameShipsOnPlanetInfoVisitor visitor);
    }
}