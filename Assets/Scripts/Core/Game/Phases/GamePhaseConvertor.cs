using Logs;

namespace Core.Game.Phases
{
    public static class GamePhaseConvertor
    {
        public static GamePhaseType ToPhaseType(int phaseId)
        {
            switch (phaseId)
            {
                case PhaseIds.InvalidPhaseId:
                    return GamePhaseType.None;
                
                case PhaseIds.GameInitializationPhaseId:
                    return GamePhaseType.Initialization;
                
                case PhaseIds.GameRegroupingPhaseId:
                    return GamePhaseType.Regroup;
                
                case PhaseIds.GameDestinyPhaseId:
                    return GamePhaseType.Destiny;
                
                default:
                    Logger.Error("Unknown phase id: " + phaseId);
                    return GamePhaseType.None;
            }
        }
    }
}