namespace Core.Game.Players.Visitors
{
    public interface IGamePlayerVisitor
    {
        void Visit(ServerGamePlayer player);
        
        void Visit(OwnerGamePlayer player);
        
        void Visit(SimpleGamePlayer player);
    }
}