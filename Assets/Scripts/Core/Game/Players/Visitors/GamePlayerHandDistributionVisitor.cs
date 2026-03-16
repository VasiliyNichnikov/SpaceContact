using Core.Game.Dto.States.Cards;

namespace Core.Game.Players.Visitors
{
    public class GamePlayerHandDistributionVisitor : IGamePlayerVisitor
    {
        private readonly PlayerHandStateData _handStateData;
        
        public GamePlayerHandDistributionVisitor(PlayerHandStateData handStateData) => 
            _handStateData = handStateData;
        
        public void Visit(ServerGamePlayer player) => 
            player.UpdateHandState(_handStateData);

        public void Visit(OwnerGamePlayer player) => 
            player.UpdateHandState(_handStateData);

        public void Visit(SimpleGamePlayer player) => 
            player.SetNumberOfCards(_handStateData.NumberOfCards);
    }
}