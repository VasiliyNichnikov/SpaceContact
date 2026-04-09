using Core.Game;
using Core.Game.Encounter;
using Core.Game.Phases;
using Core.Game.Phases.Server;
using VContainer;

namespace App.Game.Factory
{
    public static class GamePhaseFactory
    {
        public static GameInitializationPhase CreateInitializationPhase(IObjectResolver resolver, bool isServer)
        {
            var stateMachine = resolver.Resolve<GameStateMachine>();
            var playersPhaseTracker = resolver.Resolve<GamePlayersPhaseTracker>();
            IServerStateMachineNetwork? stateMachineNetwork = null;
            IGameServerEncounterManager? encounterManager = null;
            IGameServerDestinyPhaseResolver? destinyPhaseResolver = null;

            if (isServer)
            {
                stateMachineNetwork = resolver.Resolve<IServerStateMachineNetwork>();
                encounterManager = resolver.Resolve<IGameServerEncounterManager>();
                destinyPhaseResolver = resolver.Resolve<IGameServerDestinyPhaseResolver>();
            }
            
            return new GameInitializationPhase(
                stateMachine, 
                playersPhaseTracker, 
                encounterManager,
                destinyPhaseResolver,
                stateMachineNetwork);
        }
        
        public static GameDestinyPhase CreateDestinyPhase(IObjectResolver resolver, bool isServer)
        {
            var stateMachine = resolver.Resolve<GameStateMachine>();
            var clientEncounterManager = resolver.Resolve<IGameClientEncounterManager>();
            IGameServerDestinyPhaseResolver? destinyPhaseResolver = null;

            if (isServer)
            {
                destinyPhaseResolver = resolver.Resolve<IGameServerDestinyPhaseResolver>();
            }
            
            return new GameDestinyPhase(
                clientEncounterManager,
                destinyPhaseResolver,
                stateMachine);
        }
    }
}