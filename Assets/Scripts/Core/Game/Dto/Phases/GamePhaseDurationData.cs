using Core.Game.Dto.Snapshots;
using Core.Helpers;

namespace Core.Game.Dto.Phases
{
    public readonly struct GamePhaseDurationData
    {
        public readonly float RegroupPhaseDuration;
        
        public readonly float DestinyPhaseDuration;
        
        public readonly float LaunchPhaseDuration;
        
        public readonly float AlliancePhaseDuration;

        public GamePhaseDurationData(
            float regroupPhaseDuration,
            float destinyPhaseDuration,
            float launchPhaseDuration,
            float alliancePhaseDuration)
        {
            RegroupPhaseDuration = regroupPhaseDuration;
            DestinyPhaseDuration = destinyPhaseDuration;
            LaunchPhaseDuration = launchPhaseDuration;
            AlliancePhaseDuration = alliancePhaseDuration;
        }

        public bool TryGetDuration(GamePhaseType phase, out double durationInSeconds)
        {
            durationInSeconds = phase switch
            {
                GamePhaseType.Regroup => RegroupPhaseDuration,
                GamePhaseType.Destiny => DestinyPhaseDuration,
                GamePhaseType.Launch => LaunchPhaseDuration,
                GamePhaseType.Alliance => AlliancePhaseDuration,
                _ => 0
            };

            return TransitionPhaseHelper.IsTimedPhase(phase);
        }
    }
}
