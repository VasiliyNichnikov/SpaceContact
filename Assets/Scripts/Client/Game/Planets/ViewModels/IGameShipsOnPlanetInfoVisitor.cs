namespace Client.Game.Planets.ViewModels
{
    public interface IGameShipsOnPlanetInfoVisitor
    {
        void Visit(GameShipsInfoViewModel viewModel);
        
        void Visit(GameChoicePlanetToAttackViewModel viewModel);
    }
}