using Core.Game.Encounter;
using Core.Game.Phases.Client;
using Core.Game.Players;
using Core.Game.Rules;

namespace Client.Game.Planets.ViewModels
{
    public sealed class GameShipsOnPlanetInfoViewModelFactory
    {
        private readonly GameRulesChecker _rulesChecker;
        private readonly IGameClientDestinyPhaseResolver _destinyPhaseResolver;
        private readonly IGameClientEncounterManager _encounterManager;
        
        public GameShipsOnPlanetInfoViewModelFactory(
            GameRulesChecker rulesChecker,
            IGameClientDestinyPhaseResolver destinyPhaseResolver,
            IGameClientEncounterManager encounterManager)
        {
            _rulesChecker = rulesChecker;
            _destinyPhaseResolver = destinyPhaseResolver;
            _encounterManager = encounterManager;
        }
        
        public GameShipsOnPlanetInfoViewModel Create(
            int planetId,
            ulong ownerClientPlayerId,
            IGamePlayer planetPlayerOwner)
        {
            return new GameShipsOnPlanetInfoViewModel(
                planetId, 
                ownerClientPlayerId, 
                planetPlayerOwner, 
                _rulesChecker,
                _destinyPhaseResolver,
                _encounterManager);
        }
    }
}