using Core.EngineData;
using Core.Game.Players;

namespace Core.Game.Cards
{
    public class GamePlayerColorDestinyCard : IDestinyCard
    {
        public GamePlayerColorDestinyCard(IGamePlayer targetPlayer)
        {
            Description = $"Target: {targetPlayer.PlayerName}";
            BackgroundColor = targetPlayer.Color;
            TargetPlayerId = targetPlayer.PlayerId;
        }
        
        public string Description { get; }
        
        public Color BackgroundColor { get; }
        
        public ulong? TargetPlayerId { get; }
    }
}