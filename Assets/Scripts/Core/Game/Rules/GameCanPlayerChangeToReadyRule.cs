using Core.Game.Players;

namespace Core.Game.Rules
{
    public sealed class GameCanPlayerChangeToReadyRule : IGameRule
    {
        private readonly GamePlayersRegistry _playersRegistry;
        
        public GameCanPlayerChangeToReadyRule(GamePlayersRegistry playersRegistry)
        {
            _playersRegistry = playersRegistry;
        }
        
        public GameRuleType Type => 
            GameRuleType.CanPlayerChangeToReady;
        
        public bool Check(GameRuleContext context)
        {
            if (context.SelectedPlayerId == null)
            {
                return false;
            }

            var player = _playersRegistry.GetPlayerById(context.SelectedPlayerId.Value);

            return !player.IsReadyToNextPhase;
        }
    }
}