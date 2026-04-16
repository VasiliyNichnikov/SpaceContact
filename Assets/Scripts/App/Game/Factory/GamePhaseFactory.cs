using Core.Game;
using Core.Game.Cards;
using Core.Game.Encounter;
using Core.Game.Galaxy;
using Core.Game.Phases;
using Core.Game.Phases.Server;
using VContainer;

namespace App.Game.Factory
{
    public static class GamePhaseFactory
    {
        public static GameInitializationPhase CreateInitializationPhase(IObjectResolver resolver, bool isServer)
        {
            var playersPhaseTracker = resolver.Resolve<GamePlayersPhaseTracker>();
            var serverInteraction = resolver.Resolve<IGamePhaseServerInteraction>();
            var clientGalaxyManager = resolver.Resolve<IGameClientGalaxyManager>();
            var clientPlayerCardsDeckService = resolver.Resolve<IGameClientPlayerCardsDeckService>();
            GameServerPhaseTransitioner? transitioner = null;

            if (isServer)
            {
                transitioner = resolver.Resolve<GameServerPhaseTransitioner>();
            }
            
            return new GameInitializationPhase(
                playersPhaseTracker,
                clientGalaxyManager,
                clientPlayerCardsDeckService,
                serverInteraction,
                transitioner);
        }

        public static GameFirstMovePhase CreateFirstMovePhase(IObjectResolver resolver, bool isServer)
        {
            IGameServerEncounterManager? encounterManager = null;
            
            if (isServer)
            {
                encounterManager = resolver.Resolve<IGameServerEncounterManager>();
            }
            
            return new GameFirstMovePhase(encounterManager);
        }
        
        public static GameDestinyPhase CreateDestinyPhase(IObjectResolver resolver, bool isServer)
        {
            var clientEncounterManager = resolver.Resolve<IGameClientEncounterManager>();
            var phaseTimeController = resolver.Resolve<GamePhaseTimeController>();
            IGameServerDestinyPhaseResolver? destinyPhaseResolver = null;

            if (isServer)
            {
                destinyPhaseResolver = resolver.Resolve<IGameServerDestinyPhaseResolver>();
            }
            
            return new GameDestinyPhase(
                clientEncounterManager,
                destinyPhaseResolver,
                phaseTimeController);
        }
    }
}