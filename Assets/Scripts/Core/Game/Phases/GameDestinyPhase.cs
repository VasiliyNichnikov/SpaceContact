using Core.Game.Cards;
using Logs;

namespace Core.Game.Phases
{
    public class GameDestinyPhase : BasePhase
    {
        private readonly IGameServerDestinyCardController? _serverDestinyCardController;
        
        public GameDestinyPhase(
            IGameServerDestinyCardController? serverDestinyCardController, 
            GameStateMachine stateMachine) : base(stateMachine)
        {
            _serverDestinyCardController = serverDestinyCardController;
        }

        public override GamePhaseType Type => 
            GamePhaseType.Destiny;

        public override void Enter()
        {
            Logger.Warning("GameDestinyPhase.Enter");
            _serverDestinyCardController?.Init();
        }

        public override void Accept(IPhaseVisitor visitor) => 
            visitor.Visit(this);
    }
}