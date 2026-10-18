using Core.Game.Dto.Snapshots;

namespace Core.Helpers
{
    public static class TransitionPhaseHelper
    {
        public static bool TryGetNextPhase(GamePhaseType currentPhase, out GamePhaseType nextPhase)
        {
            nextPhase = currentPhase switch
            {
                GamePhaseType.Initialization => GamePhaseType.FirstMove,
                GamePhaseType.FirstMove => GamePhaseType.Regroup,
                GamePhaseType.Regroup => GamePhaseType.Destiny,
                GamePhaseType.Destiny => GamePhaseType.Launch,
                GamePhaseType.Launch => GamePhaseType.Alliance,
                _ => GamePhaseType.None
            };

            // None is initialized through a snapshot. Alliance awaits future encounter rules.
            return nextPhase != GamePhaseType.None;
        }

        public static bool IsTimedPhase(GamePhaseType phase) =>
            phase is GamePhaseType.Regroup or GamePhaseType.Destiny or GamePhaseType.Launch or GamePhaseType.Alliance;
    }
}
