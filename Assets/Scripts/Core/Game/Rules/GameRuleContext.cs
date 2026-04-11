namespace Core.Game.Rules
{
    public struct GameRuleContext
    {
        public ulong? SelectedPlayerId;
        
        public int? SelectedPlanetId;

        public static GameRuleContext CheckPlayer(ulong selectedPlayer)
        {
            var context = new GameRuleContext
            {
                SelectedPlayerId = selectedPlayer
            };

            return context;
        }

        public static GameRuleContext CheckPlanet(ulong ownerClientId, int selectedPlanetId)
        {
            var context = new GameRuleContext
            {
                SelectedPlayerId = ownerClientId,
                SelectedPlanetId = selectedPlanetId
            };
            
            return context;
        }

        public static GameRuleContext CheckPlanetToAttack(ulong initiatedByPlayerId, int planetToAttackId)
        {
            var context = new GameRuleContext
            {
                SelectedPlayerId = initiatedByPlayerId,
                SelectedPlanetId = planetToAttackId
            };
            
            return context;
        }
        
        public static GameRuleContext Empty => 
            new();
    }
}