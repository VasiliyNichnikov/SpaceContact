namespace Core.Game.Phases
{
    public interface IGamePhaseWithContext : IGamePhase
    {
        void SetContext(IPhasePayload payload);
    }
    
    public interface IGamePhaseWithContext<in T> : IGamePhaseWithContext where T : IPhasePayload
    {
        // nothing
    }
}