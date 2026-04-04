using Core.EngineData;
using Core.Game.Players;

namespace Core.Game.Cards
{
    public class DefaultPlayerColorDestinyCard : IDestinyCard
    {
        public DefaultPlayerColorDestinyCard(IGamePlayer targetPlayer)
        {
            Description = $"Target: {targetPlayer.PlayerName}";
            BackgroundColor = targetPlayer.Color;
        }
        
        public string Description { get; }
        
        public Color BackgroundColor { get; }
    }
}