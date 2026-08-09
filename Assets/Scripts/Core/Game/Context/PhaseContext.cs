using Core.Game.Dto.Snapshots;

namespace Core.Game
{
    public sealed class PhaseContext : IContext<PhaseSnapshot>
    {
        public GamePhaseType CurrentPhase { get; private set; }
        
        public double? EndTimeInSeconds { get; private set; }
        
        public double? ReadyTransitionTimeInSeconds { get; private set; }
        
        public long Revision { get; private set; }

        public void SetPhase(GamePhaseType phase, long revision, double? endTime)
        {
            CurrentPhase = phase;
            EndTimeInSeconds = endTime;
            Revision = revision;
        }

        public void SetReadyTransitionTime(double readyTransitionTimeInSc)
        {
            ReadyTransitionTimeInSeconds = readyTransitionTimeInSc;
        }

        public void ResetReadyTransitionTime()
        {
            ReadyTransitionTimeInSeconds = 0;
        }

        public void LoadSnapshot(PhaseSnapshot snapshot)
        {
            CurrentPhase = snapshot.CurrentPhase;
            EndTimeInSeconds = snapshot.EndTimeInSeconds;
            Revision = snapshot.Revision;
            ReadyTransitionTimeInSeconds = snapshot.ReadyTransitionTimeInSeconds;
        }

        public PhaseSnapshot ToSnapshot()
        {
            var snapshot = new PhaseSnapshot
            {
                CurrentPhase = CurrentPhase,
                EndTimeInSeconds = EndTimeInSeconds,
                ReadyTransitionTimeInSeconds = ReadyTransitionTimeInSeconds,
                Revision = Revision
            };
            
            return snapshot;
        }
    }
}