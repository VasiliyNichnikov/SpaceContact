namespace Core.Phases
{
    public interface IPhaseWithContext<in T> : IGamePhase where T : IPhasePayload
    {
        void SetContext(T context);
    }
}