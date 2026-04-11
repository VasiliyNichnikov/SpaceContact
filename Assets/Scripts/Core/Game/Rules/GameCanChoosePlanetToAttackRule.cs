using Core.Game.Encounter;
using Core.Game.Phases.Client;

namespace Core.Game.Rules
{
    public sealed class GameCanChoosePlanetToAttackRule : IGameRule
    {
        private readonly IGameClientEncounterManager _encounterManager;
        private readonly IGameClientDestinyPhaseResolver _destinyPhaseResolver;
        
        public GameCanChoosePlanetToAttackRule(
            IGameClientEncounterManager encounterManager,
            IGameClientDestinyPhaseResolver destinyPhaseResolver)
        {
            _encounterManager = encounterManager;
            _destinyPhaseResolver = destinyPhaseResolver;
        }
        
        public GameRuleType Type => 
            GameRuleType.CanChoosePlanetToAttack;
        
        public bool Check(GameRuleContext context)
        {
            if (context.SelectedPlanetId == null || context.SelectedPlayerId == null)
            {
                return false;
            }

            if (_encounterManager.PlanetIdToAttack != null)
            {
                return false;
            }
            
            var aggressorPlayer = _encounterManager.AggressorPlayer;

            if (aggressorPlayer == null)
            {
                return false;
            }

            if (aggressorPlayer.PlayerId != context.SelectedPlayerId)
            {
                return false;
            }

            var destinyCard = _destinyPhaseResolver.Card;
            
            if (destinyCard == null)
            {
                return false;
            }
            
            var defenderPlayer = _encounterManager.DefenderPlayer;

            if (destinyCard.TargetPlayerId == null && 
                defenderPlayer == null &&
                !aggressorPlayer.ContainsPlanet(context.SelectedPlanetId.Value))
            {
                return true;
            }

            return defenderPlayer != null && 
                   destinyCard.TargetPlayerId == defenderPlayer.PlayerId &&
                   defenderPlayer.ContainsPlanet(context.SelectedPlanetId.Value);
        }
    }
}