using Core.Game.Phases.Server;
using Logs;

namespace Core.Game.Phases
{
    public class GameDestinyPhase : BasePhase
    {
        private readonly IGameServerDestinyPhaseResolver? _serverDestinyPhaseResolver;
        
        public GameDestinyPhase(
            IGameServerDestinyPhaseResolver? serverDestinyPhaseResolver, 
            GameStateMachine stateMachine) : base(stateMachine)
        {
            _serverDestinyPhaseResolver = serverDestinyPhaseResolver;
        }

        public override GamePhaseType Type => 
            GamePhaseType.Destiny;

        public override void Enter()
        {
            Logger.Warning("GameDestinyPhase.Enter");
            _serverDestinyPhaseResolver?.ChooseDestiny();
        }

        public override void Accept(IPhaseVisitor visitor) => 
            visitor.Visit(this);
    }
}