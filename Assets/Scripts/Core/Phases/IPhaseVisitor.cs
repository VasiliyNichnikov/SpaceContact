namespace Core.Phases
{
    public interface IPhaseVisitor
    {
        void Visit(LobbyPhase phase);

        void Visit(RegroupPhase phase);
    }
}