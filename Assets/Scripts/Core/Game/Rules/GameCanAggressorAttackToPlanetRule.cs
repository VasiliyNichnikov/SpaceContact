using Core.Game.Encounter;
using Core.Game.Phases.Client;
using Core.Game.Players;

namespace Core.Game.Rules
{
    public sealed class GameCanAggressorAttackToPlanetRule : IGameRule
    {
        private readonly GamePlayersRegistry _playersRegistry;
        private readonly IGameClientDestinyPhaseResolver _destinyPhaseResolver;
        private readonly IGameClientEncounterManager _encounterManager;
        
        public GameCanAggressorAttackToPlanetRule(
            GamePlayersRegistry playersRegistry,
            IGameClientDestinyPhaseResolver destinyPhaseResolver,
            IGameClientEncounterManager encounterManager)
        {
            _playersRegistry = playersRegistry;
            _destinyPhaseResolver = destinyPhaseResolver;
            _encounterManager = encounterManager;
        }
        
        public GameRuleType Type => 
            GameRuleType.CanAggressorAttackToPlanet;
        
        public bool Check(GameRuleContext context)
        {
            if (context.SelectedPlanetId == null || context.SelectedPlayerId == null)
            {
                return false;
            }
            
            var destinyCard = _destinyPhaseResolver.Card;

            if (destinyCard == null)
            {
                return false;
            }
            
            var aggressorPlayer = _encounterManager.AggressorPlayer;

            if (aggressorPlayer == null)
            {
                return false;
            }

            var defenderPlayer = _encounterManager.DefenderPlayer;
            var selectedPlayer = _playersRegistry.GetPlayerById(context.SelectedPlayerId.Value);

            if (destinyCard.TargetPlayerId == null && 
                defenderPlayer == null &&
                aggressorPlayer.PlayerId == context.SelectedPlayerId && 
                !selectedPlayer.ContainsPlanet(context.SelectedPlanetId.Value))
            {
                return true;
            }

            if (aggressorPlayer.PlayerId == context.SelectedPlayerId &&
                defenderPlayer != null && 
                defenderPlayer.ContainsPlanet(context.SelectedPlanetId.Value) &&
                destinyCard.TargetPlayerId == defenderPlayer.PlayerId)
            {
                return true;
            }

            return false;
        }
    }
}