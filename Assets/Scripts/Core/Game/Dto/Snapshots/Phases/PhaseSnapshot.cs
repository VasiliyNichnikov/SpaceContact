using System;

namespace Core.Game.Dto.Snapshots
{
    [Serializable]
    public sealed class PhaseSnapshot
    {
        public GamePhaseType CurrentPhase = GamePhaseType.None;
        public double? EndTimeInSeconds;
        public double? ReadyTransitionTimeInSeconds;
        public long Revision;
    }
}