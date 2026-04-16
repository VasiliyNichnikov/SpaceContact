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
    }
}