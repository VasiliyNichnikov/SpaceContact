namespace Core.Game.Phases
{
    public interface IPhaseVisitor
    {
        void Visit(GameInitializationPhase phase);
    }
}