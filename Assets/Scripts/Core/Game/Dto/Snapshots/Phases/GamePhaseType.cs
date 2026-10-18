namespace Core.Game.Dto.Snapshots
{
    public enum GamePhaseType
    {
        // No active phase; startup loads Initialization through the initial snapshot.
        None = 0,
        // Untimed phases; progression waits for the entry-data ACK barrier.
        Initialization = 1,
        FirstMove = 2,
        // Timed route scaffolding; Regroup, Launch and Alliance mechanics are deferred.
        Regroup = 3,
        Destiny = 4,
        Launch = 5,
        // Terminal boundary until encounter resolution is implemented.
        Alliance = 6,
    }
}
