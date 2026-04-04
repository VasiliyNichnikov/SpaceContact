namespace Core.Game.Players.Visitors
{
    public interface IGamePlayerVisitor
    {
        void Visit(ServerGamePlayer player);
        
        void Visit(SelfGamePlayer player);
        
        void Visit(PublicGamePlayer player);
    }
}