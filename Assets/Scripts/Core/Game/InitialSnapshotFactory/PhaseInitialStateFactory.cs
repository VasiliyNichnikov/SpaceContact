using Core.Game.Dto.Snapshots;

namespace Core.Game.InitialSnapshotFactory
{
    public sealed class PhaseInitialStateFactory
    {
        public PhaseSnapshot Create(long revision)
        {
            var snapshot = new PhaseSnapshot
            {
                CurrentPhase = GamePhaseType.Initialization,
                EndTimeInSeconds = null,
                ReadyTransitionTimeInSeconds = null,
                Revision = revision
            };
            
            return snapshot;
        }
    }
}