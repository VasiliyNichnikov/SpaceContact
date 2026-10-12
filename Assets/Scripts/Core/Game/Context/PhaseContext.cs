using System;
using Core.Game.Dto.Snapshots;

namespace Core.Game
{
    public sealed class PhaseContext : IContext<PhaseSnapshot>
    {
        public event Action? PhaseChanged;

        public GamePhaseType Type { get; private set; }
        
        public double? EndTimeInSeconds { get; private set; }
        
        public double? ReadyTransitionTimeInSeconds { get; private set; }
        
        public long Revision { get; private set; }

        /// <summary>
        /// TODO: в валидации надо проверять ревизии
        /// </summary>
        /// <param name="phase"></param>
        /// <param name="revision"></param>
        /// <param name="endTime"></param>
        public void SetPhase(GamePhaseType phase, long revision, double? endTime)
        {
            Type = phase;
            EndTimeInSeconds = endTime;
            Revision = revision;
            PhaseChanged?.Invoke();
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
            Type = snapshot.CurrentPhase;
            EndTimeInSeconds = snapshot.EndTimeInSeconds;
            Revision = snapshot.Revision;
            ReadyTransitionTimeInSeconds = snapshot.ReadyTransitionTimeInSeconds;
        }

        public PhaseSnapshot ToSnapshot()
        {
            var snapshot = new PhaseSnapshot
            {
                CurrentPhase = Type,
                EndTimeInSeconds = EndTimeInSeconds,
                ReadyTransitionTimeInSeconds = ReadyTransitionTimeInSeconds,
                Revision = Revision
            };
            
            return snapshot;
        }

        internal void ReplaceState(PhaseContext prepared)
        {
            Type = prepared.Type;
            Revision = prepared.Revision;
            EndTimeInSeconds = prepared.EndTimeInSeconds;
            ReadyTransitionTimeInSeconds = prepared.ReadyTransitionTimeInSeconds;
        }
    }
}
