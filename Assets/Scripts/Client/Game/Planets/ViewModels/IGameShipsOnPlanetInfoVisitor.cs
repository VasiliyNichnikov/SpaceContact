namespace Client.Game.Planets.ViewModels
{
    public interface IGameShipsOnPlanetInfoVisitor
    {
        void Visit(GameShipsInfoViewModel viewModel);
    }
}