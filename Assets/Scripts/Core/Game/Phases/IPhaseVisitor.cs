namespace Core.Game.Phases
{
    public interface IPhaseVisitor
    {
        void Visit(GameInitializationPhase phase);
        
        void Visit(GameFirstMovePhase phase);
        
        void Visit(GameRegroupPhase phase);
        
        void Visit(GameDestinyPhase phase);
    }
}