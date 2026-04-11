using Core.Game.Cards;
using Core.Game.Encounter;
using Core.Game.Phases.Client;

namespace Core.Game.Rules
{
    public sealed class GameCanSkipDestinyCardRule : IGameRule
    {
        private readonly IGameClientDestinyPhaseResolver _destinyPhaseResolver;
        private readonly IGameClientEncounterManager _encounterManager;
        
        public GameCanSkipDestinyCardRule(
            IGameClientDestinyPhaseResolver destinyPhaseResolver,
            IGameClientEncounterManager encounterManager)
        {
            _destinyPhaseResolver = destinyPhaseResolver;
            _encounterManager = encounterManager;
        }
        
        public GameRuleType Type => 
            GameRuleType.CanSkipDestinyCard;
        
        public bool Check(GameRuleContext context)
        {
            if (context.SelectedPlayerId == null)
            {
                return false;
            }
            
            var destinyCard = _destinyPhaseResolver.Card;

            if (destinyCard is not GamePlayerColorDestinyCard)
            {
                return false;
            }

            var targetPlayerId = destinyCard.TargetPlayerId;
            
            if (targetPlayerId == null)
            {
                return false;
            }

            var aggressorPlayer = _encounterManager.AggressorPlayer;
            
            if (aggressorPlayer == null)
            {
                return false;
            }

            return targetPlayerId == aggressorPlayer.PlayerId && 
                   targetPlayerId == context.SelectedPlayerId;
        }
    }
}