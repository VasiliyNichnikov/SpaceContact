using Core.Game;
using Core.Game.Cards;
using Core.Game.Phases;
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

            if (isServer)
            {
                stateMachineNetwork = resolver.Resolve<IServerStateMachineNetwork>();
            }
            
            return new GameInitializationPhase(
                stateMachine, 
                playersPhaseTracker, 
                stateMachineNetwork);
        }
        
        public static GameDestinyPhase CreateDestinyPhase(IObjectResolver resolver, bool isServer)
        {
            var stateMachine = resolver.Resolve<GameStateMachine>();
            IGameServerDestinyCardController? destinyCardController = null;

            if (isServer)
            {
                destinyCardController = resolver.Resolve<IGameServerDestinyCardController>();
            }
            
            return new GameDestinyPhase(destinyCardController, stateMachine);
        }
    }
}