namespace Core.Game.Phases.Client
{
    public interface IGameClientPlayerReadinessEvents
    {
        void SetReady();
        
        void SetNotReady();
    }
}