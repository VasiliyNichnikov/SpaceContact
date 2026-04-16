using Core.Game.Dto.Payload;

namespace Core.Game.Phases
{
    public sealed class GameRegroupPhase : BasePhaseWithContext<GamePhaseRegroupPayload>
    {
        public GameRegroupPhase(GamePhaseTimeController phaseTimeController) : 
            base(phaseTimeController)
        {
        }

        public override GamePhaseType Type => 
            GamePhaseType.Regroup;

        public override void Accept(IPhaseVisitor visitor) => 
            visitor.Visit(this);
    }
}