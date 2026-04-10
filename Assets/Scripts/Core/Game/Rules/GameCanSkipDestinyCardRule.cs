using Core.Game.Cards;
using Core.Game.Encounter;
using Core.Game.Phases.Client;
using Core.Game.Players;

namespace Core.Game.Rules
{
    public sealed class GameCanSkipDestinyCardRule : IGameRule
    {
        private readonly IGameClientDestinyPhaseResolver _destinyPhaseResolver;
        private readonly IGameClientEncounterManager _encounterManager;
        private readonly GamePlayersRegistry _playersRegistry;
        
        public GameCanSkipDestinyCardRule(
            IGameClientDestinyPhaseResolver destinyPhaseResolver,
            IGameClientEncounterManager encounterManager,
            GamePlayersRegistry playersRegistry)
        {
            _destinyPhaseResolver = destinyPhaseResolver;
            _encounterManager = encounterManager;
            _playersRegistry = playersRegistry;
        }
        
        public GameRuleType Type => 
            GameRuleType.CanSkipDestinyCard;
        
        public bool Check(GameRuleContext context)
        {
            var ownerPlayer = _playersRegistry.GetOwnerWithError(silently:true);

            if (ownerPlayer == null)
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
                   targetPlayerId == ownerPlayer.PlayerId;
        }
    }
}