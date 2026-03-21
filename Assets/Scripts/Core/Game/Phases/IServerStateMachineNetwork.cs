namespace Core.Game.Phases
{
    public interface IServerStateMachineNetwork
    {
        void ServerTransitionTo<TPhase>(IPhasePayload? payload = null)
            where TPhase : IGamePhase;
    }
}