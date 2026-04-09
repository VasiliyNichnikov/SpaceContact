namespace Core.Game.Rules
{
    public struct GameRuleContext
    {
        public ulong? SelectedPlayerId;

        public static GameRuleContext CheckPlayer(ulong selectedPlayer)
        {
            var context = new GameRuleContext
            {
                SelectedPlayerId = selectedPlayer
            };

            return context;
        }
        
        public static GameRuleContext Empty => 
            new();
    }
}