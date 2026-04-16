namespace Core.Game.Phases
{
    /// <summary>
    /// Не забудь после добавления Id,
    /// указать его в GamePhaseConvertor
    /// </summary>
    public static class PhaseIds
    {
        public const int InvalidPhaseId = int.MinValue;
        public const byte GameInitializationPhaseId = 100;
        public const byte GameFirstMovePhaseId = 101;
        public const byte GameRegroupPhaseId = 102;
        public const byte GameRegroupingPhaseId = 103;
        public const byte GameDestinyPhaseId = 104;
    }
}