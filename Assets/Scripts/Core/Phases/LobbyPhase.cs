using Logs;

namespace Core.Phases
{
    public class LobbyPhase : BasePhase
    {
        public LobbyPhase(GameStateMachine stateMachine) : base(stateMachine)
        {
        }

        public override void Enter()
        {
            Logger.Log("LobbyPhase.Enter()");
        }

        public override void Accept(IPhaseVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}