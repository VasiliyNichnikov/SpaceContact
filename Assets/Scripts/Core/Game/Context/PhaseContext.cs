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

        public void SetPhase(GamePhaseType phase, long revision)
        {
            Type = phase;
            EndTimeInSeconds = null;
            ReadyTransitionTimeInSeconds = null;
            Revision = revision;
            PhaseChanged?.Invoke();
        }

        public void StartTimer(double endTimeInSeconds)
        {
            EndTimeInSeconds = endTimeInSeconds;
        }

        public void SetReadyTransitionTime(double readyTransitionTimeInSeconds)
        {
            ReadyTransitionTimeInSeconds = readyTransitionTimeInSeconds;
        }

        public void ResetReadyTransitionTime()
        {
            ReadyTransitionTimeInSeconds = null;
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
