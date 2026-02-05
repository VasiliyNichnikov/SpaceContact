namespace Core.Phases
{
    public interface IGamePhase
    {
        void Enter();

        void Exit();
        
        void Update();
        
        void Accept(IPhaseVisitor visitor);
    }
}