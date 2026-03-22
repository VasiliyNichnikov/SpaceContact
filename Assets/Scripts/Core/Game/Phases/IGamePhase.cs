namespace Core.Game.Phases
{
    public interface IGamePhase
    {
        GamePhaseType Type { get; }
        
        void Enter();

        void Exit();
        
        void Update();
        
        void Accept(IPhaseVisitor visitor);
    }
}