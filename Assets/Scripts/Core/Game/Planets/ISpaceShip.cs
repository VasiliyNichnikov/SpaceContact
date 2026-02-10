namespace Core.Game.Planets
{
    public interface ISpaceShip
    {
        int Id { get; }
        
        ulong OwnerId { get; }
    }
}